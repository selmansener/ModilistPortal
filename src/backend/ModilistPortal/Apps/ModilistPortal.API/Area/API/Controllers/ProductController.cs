using MediatR;

using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("UploadProductExcel")]
        public async Task<IActionResult> UploadProductExcel([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UploadProductExcel
            {
                AccountId = User.GetUserId(),
                File = file,
            });

            return Ok();
        }
    }
}
