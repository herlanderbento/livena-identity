using Livena.Identity.Api.ApiModels.User;
using Livena.Identity.Api.Authorization;
using Livena.Identity.Api.Presenters;
using Livena.Identity.Api.Validators;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Application.UseCases.User.CreateUser;
using Livena.Identity.Application.UseCases.User.DeleteUser;
using Livena.Identity.Application.UseCases.User.GetUser;
using Livena.Identity.Application.UseCases.User.ListUsers;
using Livena.Identity.Application.UseCases.User.SendVerificationCode;
using Livena.Identity.Application.UseCases.User.UpdateUser;
using Livena.Identity.Application.UseCases.User.VerifyAccount;
using Livena.Identity.Domain.Shared.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livena.Identity.Api.Controllers;

[ApiController]
[Route("/api/v1/users")]
public class UsersController(IMediator mediator, RequestValidator requestValidator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly RequestValidator _requestValidator = requestValidator;

    [HttpPost]
    [ProducesResponseType(typeof(ApiPresenter<UserOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserInput request,
        CancellationToken cancellationToken
    )
    {
        _requestValidator.Validate(request, cancellationToken);
        var output = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(
            nameof(Create),
            new { output.Id },
            new ApiPresenter<UserOutput>(output)
        );
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin}")]
    [ProducesResponseType(typeof(ListUsersOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListUsersInput();
        if (page is not null)
            input.Page = page.Value;
        if (perPage is not null)
            input.PerPage = perPage.Value;
        if (!String.IsNullOrWhiteSpace(search))
            input.Search = search;
        if (!String.IsNullOrWhiteSpace(sort))
            input.Sort = sort;
        if (dir is not null)
            input.Dir = dir.Value;

        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiPresenterList<UserOutput>(output));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin}, {Roles.User}")]
    [ProducesResponseType(typeof(ApiPresenter<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = await _mediator.Send(new GetUserInput(id), cancellationToken);
        return Ok(new ApiPresenter<UserOutput>(output));
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(ApiPresenter<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUserApiInput request,
        CancellationToken cancellationToken
    )
    {
        var input = new UpdateUserInput(
            id,
            request.Email,
            request.Phone,
            request.Birthday,
            request.IsActive
        );

        _requestValidator.Validate(input, cancellationToken);
        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiPresenter<UserOutput>(output));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteUserInput(id);
        await _mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpPost("send-verification-code")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendVerificationCode(
        [FromBody] SendVerificationCodeInput request,
        CancellationToken cancellationToken
    )
    {
        _requestValidator.Validate(request, cancellationToken);
        await _mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpPost("verify-account")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Verify(
        [FromBody] VerifyAccountInput request,
        CancellationToken cancellationToken
    )
    {
        _requestValidator.Validate(request, cancellationToken);
        await _mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
