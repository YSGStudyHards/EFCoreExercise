using EFCoreGenericRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCoreGenericRepository.Implementations
{
    /// <summary>
    /// 针对单一 TDbContext 的工作单元实现。
    /// 如果项目存在多个 DbContext，需要为每个上下文分别注册一个 UnitOfWork<TContext>。
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
        /// 在 UoW 中的仓储实例：禁止自动保存，写操作只更改状态/ChangeTracker
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
        /// 事务包装器：自动开启/提交/回滚
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
                // 嵌套场景直接执行
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
            // DbContext 由 DI Scoped 管理，不主动释放
        }
    }
}