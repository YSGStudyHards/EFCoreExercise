using System.Data;
using EFCoreGenericRepository.Interfaces;
using EFCoreGenericRepository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EFCoreGenericRepository.Implementations
{
    /// <summary>
    /// 通用仓储实现类
    /// 主要职责：
    /// 1. 对 EF Core 的常用 CRUD、查询、分页、事务等操作进行统一封装。
    /// 2. 降低业务层对具体 DbContext 的耦合，利于测试与维护。
    /// 3. 预留扩展点：审计字段填充、软删除过滤、多租户过滤、读写分离、缓存等。
    /// 
    /// 线程安全说明：
    /// - Repository 以 Scoped 生命周期注册（与 DbContext 一致）时，默认不跨线程共享；不要在多线程中并发使用同一个实例。
    /// </summary>
    public class Repository : IRepository
    {
        private readonly DbContext _dbContext;

        /// <summary>
        /// 初始化仓储实例。
        /// </summary>
        /// <param name="context">
        /// 数据库上下文实例。由依赖注入容器（DI）管理生命周期。
        /// 推荐：在 Startup/Program 中使用 AddDbContext 注册为 Scoped。
        /// </param>
        /// <exception cref="ArgumentNullException">当 <paramref name="context"/> 为空时抛出异常</exception>
        public Repository(DbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }


        #region 增删改操作

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">要添加的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            int count = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">要添加的实体列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) throw new ArgumentException("实体集合不能为空", nameof(entities));

            var entityList = entities.ToList();
            await _dbContext.Set<TEntity>().AddRangeAsync(entityList, cancellationToken).ConfigureAwait(false);
            int count = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">要更新的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().Update(entity);
            int count = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">要更新的实体列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.ToList();
            _dbContext.Set<TEntity>().UpdateRange(entityList);
            int count = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">要删除的实体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().Remove(entity);
            int count = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="id">主键id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> DeleteByIdAsync<TEntity>(object id, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entity = await GetByIdAsync<TEntity>(id, cancellationToken).ConfigureAwait(false);
            if (entity != null)
            {
                return await DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">要删除的实体列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            int count = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region 数据查询操作

        /// <summary>
        /// 获取实体的可查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>可继续追加 Where/Select/Include 等的查询对象。</returns>
        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return _dbContext.Set<TEntity>();
        }

        /// <summary>
        /// 根据主键 ID 获取单个实体
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<TEntity> GetByIdAsync<TEntity>(object id, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// 按条件获取第一个匹配实体，若不存在则返回 null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">为空时抛出异常</exception>
        public async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (queryExpression == null)
                throw new ArgumentNullException(nameof(queryExpression));

            return await _dbContext.Set<TEntity>().Where(queryExpression).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 按条件获取第一个匹配实体（可包含导航属性），若不存在则返回 null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="includes">导航属性 Include 构建委托；可链式调用 ThenInclude</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">为空时抛出异常</exception>
        public async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (queryExpression == null) throw new ArgumentNullException(nameof(queryExpression));

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.Where(queryExpression).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 获取所有实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetAllAsync<TEntity>(CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await _dbContext.Set<TEntity>().AsQueryable().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetListAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await _dbContext.Set<TEntity>().AsQueryable().Where(queryExpression).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件获取实体列表（包含导航属性）
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="includes">导航属性 Include 构建委托；可链式调用 ThenInclude</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetListAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.Where(queryExpression).ToListAsync(cancellationToken);
        }

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
        public async Task<PagedResult<TEntity>> GetPagedAsync<TEntity>(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> queryExpression = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (queryExpression != null)
            {
                query = query.Where(queryExpression);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TEntity>
            {
                PageDatas = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 检查是否存在满足条件的实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await _dbContext.Set<TEntity>().AsQueryable()
                .Where(queryExpression)
                .AnyAsync(cancellationToken);
        }

        /// <summary>
        /// 获取满足条件的实体数量
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="queryExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<int> CountAsync<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (queryExpression != null)
            {
                query = query.Where(queryExpression);
            }

            return await query.CountAsync(cancellationToken);
        }

        #endregion
    }
}
