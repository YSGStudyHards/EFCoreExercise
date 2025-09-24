using EFCoreGenericRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCoreGenericRepository.Implementations
{
    /// <summary>
    /// ��Ե�һ TDbContext �Ĺ�����Ԫʵ�֡�
    /// �����Ŀ���ڶ�� DbContext����ҪΪÿ�������ķֱ�ע��һ�� UnitOfWork<TContext>��
    /// </summary>
    public sealed class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private Repository<TDbContext> _repository;
        private bool _disposed;

        public UnitOfWork(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// �� UoW �еĲִ�ʵ������ֹ�Զ����棬д����ֻ����״̬/ChangeTracker
        /// </summary>
        public IRepository<TDbContext> Repository =>
            _repository ??= new Repository<TDbContext>(_dbContext, autoSaveChanges: false);

        public TDbContext DbContext => _dbContext;

        public bool HasActiveTransaction => _transaction != null;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null) return;
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        /// <summary>
        /// �����װ�����Զ�����/�ύ/�ع�
        /// </summary>
        public async Task ExecuteInTransactionAsync(
            Func<IRepository<TDbContext>, Task> action,
            CancellationToken cancellationToken = default)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            if (!HasActiveTransaction)
            {
                await BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await action(Repository).ConfigureAwait(false);
                    await CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
            else
            {
                // Ƕ�׳���ֱ��ִ��
                await action(Repository).ConfigureAwait(false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            if (_transaction != null)
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
                _transaction = null;
            }
            // DbContext �� DI Scoped �����������ͷ�
        }
    }
}