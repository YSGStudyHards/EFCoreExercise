using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace EFCoreGenericRepository.Interfaces
{
    /// <summary>
    /// 通用仓储接口，继承查询仓储并提供写操作
    /// </summary>
    public interface IRepository : IQueryRepository
    {
        /// <summary>
        /// 开始数据库事务（默认隔离级别）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>数据库事务对象</returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行事务操作
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">要执行的操作</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">要添加的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>添加的实体</returns>
        Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">要添加的实体列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">要更新的实体</param>
        /// <returns>更新的实体</returns>
        TEntity Update<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">要更新的实体列表</param>
        void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">要删除的实体</param>
        void Delete<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="id">实体ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task DeleteByIdAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TKey>;

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">要删除的实体列表</param>
        void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
