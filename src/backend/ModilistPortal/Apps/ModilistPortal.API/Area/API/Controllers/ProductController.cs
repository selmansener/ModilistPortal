using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ModilistPortal.API.Configuration;
using ModilistPortal.API.Models;
using ModilistPortal.Business.CQRS.ProductDomain.Commands;
using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.CQRS.ProductDomain.Queries;
using ModilistPortal.Business.DTOs;
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

        [Authorize(nameof(AuthorizationPermissions.Products))]
        [HttpGet("QueryUploadHistory")]
        [DynamicQuery]
        [ProducesResponseType(typeof(ResponseModel<DQBResultDTO<ProductExcelUploadDTO>>), 200)]
        public async Task<IActionResult> QueryUploadHistory([FromQuery] DynamicQueryOptions dqb, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new QueryProductExcelUploadHistory(User.GetUserId(), dqb), cancellationToken);

            return Ok(new ResponseModel<DQBResultDTO<ProductExcelUploadDTO>>(result));
        }

        [Authorize(nameof(AuthorizationPermissions.Products))]
        [HttpGet("QueryUploadHistoryDetails/{productExcelUploadId}")]
        [DynamicQuery]
        [ProducesResponseType(typeof(ResponseModel<DQBResultDTO<QueryProductExcelRowDTO>>), 200)]
        public async Task<IActionResult> QueryUploadHistory(int productExcelUploadId, [FromQuery] DynamicQueryOptions dqb, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new QueryProductExcelUploadHistoryDetails(productExcelUploadId, User.GetUserId(), dqb), cancellationToken);

            return Ok(new ResponseModel<DQBResultDTO<QueryProductExcelRowDTO>>(result));
        }
    }
}
