using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ModilistPortal.API.Configuration;
using ModilistPortal.Business.CQRS.ProductDomain.Commands;
using ModilistPortal.Infrastructure.Shared.Exntensions;

namespace ModilistPortal.API.Area.API.Controllers
{
    public class ProductController : APIBaseController
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(nameof(AuthorizationPermissions.Products))]
        [HttpPost("UploadProductExcel")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UploadProductExcel(IFormFile file, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UploadProductExcel
            {
                AccountId = User.GetUserId(),
                File = file,
            }, cancellationToken);

            return Ok();
        }
    }
}
