using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using ModilistPortal.Data.Transactions;

namespace ModilistPortal.Business.PipelineBehaviors
{
    internal class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly ITransactionManager _transactionManager;

        public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, ITransactionManager transactionManager)
        {
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response;
            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);
                response = await next();
                await _transactionManager.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                try
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                }
                catch (Exception rollbackException)
                {
                    _logger.LogError(rollbackException.Message);
                    throw;
                }

                throw;
            }
            finally
            {
                _transactionManager.Dispose();
            }

            return response;
        }
    }
}
