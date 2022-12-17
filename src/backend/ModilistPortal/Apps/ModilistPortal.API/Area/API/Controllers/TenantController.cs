using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ModilistPortal.API.Configuration;
using ModilistPortal.API.Models;
using ModilistPortal.Business.CQRS.TenantDomain.Commands;
using ModilistPortal.Business.CQRS.TenantDomain.DTOs;
using ModilistPortal.Business.CQRS.TenantDomain.Queries;
using ModilistPortal.Infrastructure.Shared.Exntensions;

namespace ModilistPortal.API.Area.API.Controllers
{
    public class TenantController : APIBaseController
    {
        private readonly IMediator _mediator;

        public TenantController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(nameof(AuthorizationPermissions.Accounts))]
        [HttpGet("Get")]
        [ProducesResponseType(typeof(ResponseModel<TenantDTO>), 200)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var account = await _mediator.Send(new GetTenant { AccountId = User.GetUserId() }, cancellationToken);

            return Ok(new ResponseModel<TenantDTO>(account));
        }

        [Authorize(nameof(AuthorizationPermissions.Accounts))]
        [HttpPost("Upsert")]
        [ProducesResponseType(typeof(ResponseModel<TenantDTO>), 200)]
        public async Task<IActionResult> Upsert(UpsertTenant upsertTenant, CancellationToken cancellationToken)
        {
            upsertTenant.AccountId = User.GetUserId();

            var tenant = await _mediator.Send(upsertTenant, cancellationToken);

            return Ok(new ResponseModel<TenantDTO>(tenant));
        }
    }
}
