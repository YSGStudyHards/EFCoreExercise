using Microsoft.EntityFrameworkCore;

namespace EFCoreGenericRepository.Interfaces
{
    /// <summary>
    /// 非泛型工作单元抽象接口
    /// 用于协调一次业务操作内对同一 DbContext 的多次数据写入，
    /// 以便集中控制事务（显式或隐式）与提交时机，减少散落的 SaveChanges 调用。
    /// </summary>
    /// <remarks>
    /// 使用指引：
    /// 1. 只做一次简单写操作：可直接用仓储（无需 UoW）。
    /// 2. 多写操作需原子性：调用 <see cref="BeginTransactionAsync"/> -> (多次写) -> <see cref="CommitAsync"/>。
    /// 3. 避免长事务：尽量将事务范围缩到最小（准备数据放在事务外）。
    /// 4. DbContext / UnitOfWork 均为 Scoped（典型 Web 请求内使用），非线程安全；不要跨线程并发使用同一实例。
    /// </remarks>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// 显式开启数据库事务。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        /// <remarks>
        /// 1. 已有活动事务时再次调用将被忽略（可根据需要改为抛异常的严格模式）。
        /// 2. 若不调用本方法直接执行 <see cref="CommitAsync"/>：
        ///    - EF Core 在一次 <c>SaveChanges()</c> 内部仍使用隐式事务（保证单批操作的最小原子性），
        ///      但无法覆盖多次分散的 SaveChanges。
        /// 3. 建议：凡需确保“多条写操作整体成功或失败”的场景务必显式调用。
        /// </remarks>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 提交当前事务或（无显式事务时）执行一次保存。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务（不返回受影响行数，如需可扩展为 Task&lt;int&gt;）</returns>
        /// <remarks>
        /// 行为分支：
        /// 已调用 <see cref="BeginTransactionAsync"/>：调用内部 <c>SaveChangesAsync()</c> 后执行数据库事务 Commit。
        /// 未调用 <see cref="BeginTransactionAsync"/>：仅执行一次 <c>SaveChangesAsync()</c>（使用 EF Core 隐式事务）。
        ///
        /// 注意：
        /// 1. 若在 Commit 过程中 <c>SaveChangesAsync</c> 抛异常，建议上层捕获后调用 <see cref="RollbackAsync"/>（某些实现会在内部自动 Rollback）。
        /// 2. 调用顺序错误（例如忘记 Begin）不会抛异常（当前默认宽松），可根据团队规范改为严格模式（直接抛出）。
        /// 3. 重复调用 Commit：第二次因已无活动事务，只会再次触发一次 SaveChanges（可能不是你期望的语义，请谨慎）。
        /// </remarks>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 回滚当前活动事务（若存在）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        /// <remarks>
        /// 1. 未开始事务时调用将被忽略（无副作用）。
        /// 2. 回滚仅影响数据库端已写入但未提交的数据；内存中跟踪的实体状态仍保留（可按需手动 Detach）。
        /// 3. 建议在捕获到业务 / 数据异常时调用，确保不提交部分成功的操作。
        /// </remarks>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 直接调用 DbContext.SaveChangesAsync（不管理事务生命周期）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数（由 EF Core 返回）</returns>
        /// <remarks>
        /// 使用场景：
        /// 1. 不需要跨多次写操作的原子性，仅提交当前已累积变更。
        /// 2. 在显式事务中调用：不会自动 Commit，仍需显式调用 <see cref="CommitAsync"/>。
        ///
        /// 区别：
        /// - <see cref="SaveChangesAsync"/>：只做一次持久化，不结束事务。
        /// - <see cref="CommitAsync"/>：会执行 SaveChanges（若无错误）并提交事务（如果存在）。
        /// </remarks>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 泛型工作单元抽象接口
    /// 特定 DbContext 的工作单元扩展接口
    /// </summary>
    /// <typeparam name="TDbContext">关联的 DbContext 类型</typeparam>
    public interface IUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        /// <summary>
        /// 与当前工作单元共享的仓储实例（使用同一 DbContext）
        /// 写操作在未显式 Commit 前不会自动调用 SaveChanges（典型设置为 autoSaveChanges:false）
        /// </summary>
        IRepository<TDbContext> Repository { get; }

        /// <summary>
        /// 直接访问底层 DbContext（谨慎使用，避免泄漏对外部层次结构的依赖）
        /// </summary>
        TDbContext DbContext { get; }

        /// <summary>
        /// 是否存在活动事务
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// 简化事务模式的包装方法：自动 Begin / Commit / Rollback
        /// </summary>
        /// <param name="action">使用仓储执行的异步操作委托</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        /// <remarks>
        /// 1. 若当前无事务：自动开启事务，执行委托，成功则 Commit，发生异常自动 Rollback 并重新抛出。
        /// 2. 若当前已有事务（嵌套调用）：不再重复开启事务，仅复用现有事务。
        /// 3. 适合在应用服务层封装“一个业务用例 = 一次事务”逻辑，减少模板代码。
        /// </remarks>
        Task ExecuteInTransactionAsync(Func<IRepository<TDbContext>, Task> action, CancellationToken cancellationToken = default);
    }
}