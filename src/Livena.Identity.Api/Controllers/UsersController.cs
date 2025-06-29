using Livena.Identity.Api.Presenters;
using Livena.Identity.Api.Validators;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Application.UseCases.User.CreateUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;

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
        return CreatedAtAction(nameof(Create), new { output.Id }, new ApiPresenter<UserOutput>(output));
    }

}