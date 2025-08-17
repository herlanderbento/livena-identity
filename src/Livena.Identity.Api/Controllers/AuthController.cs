using System.Security.Claims;
using System.Text.Json;
using Livena.Identity.Api.Authorization;
using Livena.Identity.Api.Helpers;
using Livena.Identity.Api.Presenters;
using Livena.Identity.Api.Validators;
using Livena.Identity.Application.UseCases.User.Authenticate;
using Livena.Identity.Application.UseCases.User.ForgotPassword;
using Livena.Identity.Application.UseCases.User.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livena.Identity.Api.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController(IMediator mediator, RequestValidator requestValidator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly RequestValidator _requestValidator = requestValidator;

    [HttpPost]
    [ProducesResponseType(typeof(ApiPresenter<AuthenticateOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate(
        [FromBody] AuthenticateInput request,
        CancellationToken cancellationToken
    )
    {
        var input = new AuthenticateInput(request.Username, request.Password);
        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiPresenter<AuthenticateOutput>(output));
    }

    [HttpDelete("logout")]
    [Authorize(Roles = $"{Roles.User},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var (username, accessToken, expiresAt) = JwtTokenHelper.ExtractTokenInfo(
                User,
                authHeader
            );

            var input = new LogoutInput(username, accessToken, expiresAt);
            _requestValidator.Validate(input, cancellationToken);
            await _mediator.Send(input, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = "Invalid Token",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                }
            );
        }
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordInput request,
        CancellationToken cancellationToken
    )
    {
        _requestValidator.Validate(request, cancellationToken);
        await _mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
