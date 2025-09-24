using EFCoreGenericRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCoreGenericRepository.Implementations
{
    /// <summary>
    /// 工作单元（Unit of Work）泛型实现：聚合单个 <typeparamref name="TDbContext"/> 的数据写入操作，
    /// 控制事务边界与提交时机，减少在业务代码中散落的多次 <c>SaveChangesAsync()</c> 调用。
    /// </summary>
    /// <typeparam name="TDbContext">具体的 DbContext 类型（需在 DI 中注册为 Scoped）</typeparam>
    public sealed class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private Repository<TDbContext> _repository;
        private bool _disposed;

        /// <summary>
        /// 构造工作单元实例。
        /// </summary>
        /// <param name="dbContext">由依赖注入提供的 DbContext 实例（Scoped 生命周期）</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="dbContext"/> 为空时抛出</exception>
        public UnitOfWork(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// 该工作单元内部使用的仓储实例
        /// </summary>
        /// <remarks>
        /// 使用同一个 <see cref="DbContext"/>；
        /// 构造时指定 <c>autoSaveChanges:false</c>，写操作只变更跟踪状态，不立即落库；
        /// 需通过 <see cref="SaveChangesAsync"/> 或 <see cref="CommitAsync"/> 触发真正持久化。
        /// </remarks>
        public IRepository<TDbContext> Repository => _repository ??= new Repository<TDbContext>(_dbContext, autoSaveChanges: false);

        /// <summary>
        /// 暴露底层 DbContext（仅在需要访问 EF Core 特性 / 原生 API 时使用，谨慎避免泄漏到表示层）
        /// </summary>
        public TDbContext DbContext => _dbContext;

        /// <summary>
        /// 是否存在活跃事务
        /// </summary>
        public bool HasActiveTransaction => _transaction != null;

        /// <summary>
        /// 显式开启数据库事务（若已存在活动事务则忽略本次调用）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <remarks>
        /// 建议：凡需要保证“多次写操作整体成功或全部失败”的业务步骤，应显式调用。
        /// 未显式开启而直接 Commit，只能获得单次保存的原子性（EF Core 隐式事务），无法涵盖多个分散的写入步骤。
        /// </remarks>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null) return;
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 保存当前 DbContext 跟踪的所有变更（不负责提交/结束事务）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        /// <remarks>
        /// 场景：
        /// 1. 仅需立即持久化当前变更，但仍计划稍后继续在同一事务中操作（需先 <see cref="BeginTransactionAsync"/>）。
        /// 2. 不需要事务，只是简单提交一次。
        /// 注意：在活动事务中调用该方法不会结束事务；需后续显式调用 <see cref="CommitAsync"/>。
        /// </remarks>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 提交当前事务；若未显式开启事务，则执行一次普通保存
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <remarks>
        /// 行为：
        /// 有事务：执行 <c>SaveChangesAsync</c> 后 Commit，并释放事务对象。
        /// 无事务：仅执行一次 <c>SaveChangesAsync</c>（EF Core 内部仍包裹隐式短事务）。
        /// 建议：需要事务语义的场景务必在调用前使用 <see cref="BeginTransactionAsync"/>。
        /// </remarks>
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                // 宽松模式：未显式事务时仍允许直接保存（可根据团队规范改为抛异常严格模式）
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            else
            {
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                await _transaction.DisposeAsync().ConfigureAwait(false);
                _transaction = null;
            }
        }

        /// <summary>
        /// 回滚当前活动事务；无事务时静默返回
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <remarks>
        /// 回滚后：内存中的已跟踪实体状态不会自动还原为数据库最新值，可按需手动刷新或 Detach。
        /// </remarks>
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        /// <summary>
        /// 事务包装器：自动执行“开启事务 → 执行业务委托 → 成功提交 / 失败回滚”流程
        /// </summary>
        /// <param name="action">包含写操作的委托（使用提供的仓储实例）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="action"/> 为空时抛出</exception>
        /// <remarks>
        /// 嵌套行为：若当前已有活动事务，则不会重新开启事务，仅复用该事务环境。
        /// 使用场景：在应用服务中快速包裹一个用例的原子操作，减少模板样板代码。
        /// </remarks>
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
                // 复用已有事务（避免嵌套重复开启）
                await action(Repository).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 释放工作单元内部占用的非托管/托管资源（主要是活动事务）
        /// </summary>
        /// <remarks>
        /// 不会释放 <see cref="DbContext"/> 实例，其生命周期由 DI Scoped 管理。
        /// 多次调用安全（幂等）。
        /// </remarks>
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            if (_transaction != null)
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
                _transaction = null;
            }
        }
    }
}