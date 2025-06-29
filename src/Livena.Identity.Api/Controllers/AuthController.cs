using Livena.Identity.Api.Presenters;
using Livena.Identity.Application.UseCases.User.Authenticate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Livena.Identity.Api.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(ApiPresenter<AuthenticateOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate(
        [FromBody] AuthenticateInput request,
        CancellationToken cancellationToken
    )
    {
        var input = new AuthenticateInput(
            request.Username,
            request.Password
        );
        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiPresenter<AuthenticateOutput>(output));
    }
}