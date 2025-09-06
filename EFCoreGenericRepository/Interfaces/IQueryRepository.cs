using EFCoreGenericRepository.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EFCoreGenericRepository.Interfaces
{
    /// <summary>
    /// 查询仓储接口，提供只读查询操作
    /// </summary>
    public interface IQueryRepository
    {
        /// <summary>
        /// 获取实体的可查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>返回IQueryable对象</returns>
        IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class;

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="id">主键id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体对象</returns>
        Task<TEntity> GetByIdAsync<TEntity>(object id, CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 按条件获取第一个匹配实体，若不存在则返回 null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体对象</returns>
        Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 根据条件获取单个实体（包含导航属性）
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="includes">导航属性包含表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体对象</returns>
        Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 获取所有实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        Task<List<TEntity>> GetAllAsync<TEntity>(CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        Task<List<TEntity>> GetListAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 根据条件获取实体列表（包含导航属性）
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="includes">导航属性包含表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        Task<List<TEntity>> GetListAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 通用分页查询（支持条件 + 排序）
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="pageIndex">页索引（从0开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="queryExpression">条件（可为空）</param>
        /// <param name="orderBy">排序（可为空，未提供时使用数据库默认顺序，通常不建议）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<PagedResult<TEntity>> GetPagedAsync<TEntity>(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> queryExpression = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 检查是否存在满足条件的实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// 获取满足条件的实体数量
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体数量</returns>
        Task<int> CountAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression = null,
            CancellationToken cancellationToken = default)
            where TEntity : class;
    }
}
