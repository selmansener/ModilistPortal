using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;

using ModilistPortal.Data.DataAccess;

namespace ModilistPortal.Data.Transactions
{
    public interface ITransactionManager : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken);

        Task CommitTransactionAsync(CancellationToken cancellationToken);

        Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }

    internal class TransactionManager : ITransactionManager
    {
        private readonly ModilistPortalDbContext _dbContext;
        private IDbContextTransaction _dbTransaction;
        private readonly bool _shouldDisposeDbContext;

        public TransactionManager(ModilistPortalDbContext dbContext, bool shouldDisposeDbContext = true)
        {
            _dbContext = dbContext;
            _shouldDisposeDbContext = shouldDisposeDbContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_dbTransaction == null)
            {
                _dbTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            }
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_dbTransaction == null)
            {
                throw new InvalidOperationException("A DbTransaction is not stared.");
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbTransaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_dbTransaction == null)
            {
                throw new InvalidOperationException("A DbTransaction is not stared.");
            }

            await _dbTransaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbTransaction != null)
                {
                    _dbTransaction.Dispose();
                }

                if (_shouldDisposeDbContext)
                {
                    _dbContext.Dispose();
                }
            }
        }
    }
}
