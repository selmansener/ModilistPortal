using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModilistPortal.API.Configuration;
using ModilistPortal.Business.CQRS.AccountDomain.Commands;
using ModilistPortal.Business.CQRS.AccountDomain.DTOs;
using ModilistPortal.Business.CQRS.AccountDomain.Queries;
using ModilistPortal.Infrastructure.Shared.Exntensions;

using System.Security.Principal;

namespace ModilistPortal.API.Area.API.Controllers
{
    public class AccountController : APIBaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(nameof(AuthorizationPermissions.Accounts))]
        [HttpGet("Get")]
        [ProducesResponseType(typeof(AccountDTO), 200)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var account = await _mediator.Send(new GetAccount { Id = User.GetUserId() }, cancellationToken);

            return Ok(account);
        }

        [HttpGet("Verify/{email}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Create(string email, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new VerifyAccount
            {
                Email = email,
            });

            return Redirect(response.RedirectUrl);
        }

        [Authorize(nameof(AuthorizationPermissions.Accounts))]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(AccountDTO), 200)]
        public async Task<IActionResult> Create(CreateAccount input, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(input, cancellationToken);

            return Ok(response);
        }

        [Authorize(nameof(AuthorizationPermissions.Accounts))]
        [HttpPost("Activate")]
        [ProducesResponseType(typeof(AccountDTO), 200)]
        public async Task<IActionResult> Activate(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ActivateAccount
            {
                AccountId = User.GetUserId()
            }, cancellationToken);

            return Ok(response);
        }
    }
}
