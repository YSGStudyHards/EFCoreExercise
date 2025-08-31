using EFCoreGenericRepository.Interfaces;
using EFCoreGenericRepository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using System.Data;

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
        private readonly DbContext _context;

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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region 事务管理操作

        /// <summary>
        /// 开始数据库事务（使用数据库提供程序的默认隔离级别）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>数据库事务对象</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// 执行事务操作
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">要执行的操作</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> action,
            CancellationToken cancellationToken = default)
        {
            // 已经处于外部事务中：直接执行，不再开启新事务。
            if (_context.Database.CurrentTransaction != null)
            {
                return await action();
            }

            // 使用 EF Core 的执行策略来处理连接弹性
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await BeginTransactionAsync(cancellationToken);
                try
                {
                    var result = await action();
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
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
            return _context.Set<TEntity>();
        }

        /// <summary>
        /// 根据主键 ID 获取单个实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型（需实现 <see cref="IEntity{TKey}"/>）</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="id">主键值</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<TEntity> GetByIdAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TKey>
        {
            return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// 按条件获取第一个匹配实体，若不存在则返回 null。
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="predicate">过滤条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">为空时抛出异常</exception>
        public async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 按条件获取第一个匹配实体（可包含导航属性），若不存在则返回 null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="predicate">过滤条件表达式</param>
        /// <param name="includes">导航属性 Include 构建委托；可链式调用 ThenInclude</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">为空时抛出异常</exception>
        public async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.Where(predicate).FirstOrDefaultAsync(cancellationToken);
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
            return await _context.Set<TEntity>().AsQueryable().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="predicate">过滤条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetListAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await _context.Set<TEntity>().AsQueryable().Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件获取实体列表（包含导航属性）
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="predicate">过滤条件表达式</param>
        /// <param name="includes">导航属性 Include 构建委托；可链式调用 ThenInclude</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetListAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 通用分页查询（支持条件 + 排序）
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="pageIndex">页索引（从0开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="predicate">条件（可为空）</param>
        /// <param name="orderBy">排序（可为空，未提供时使用数据库默认顺序，通常不建议）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<PagedResult<TEntity>> GetPagedAsync<TEntity>(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
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
        /// <param name="predicate">过滤条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await _context.Set<TEntity>().AsQueryable()
                .Where(predicate)
                .AnyAsync(cancellationToken);
        }

        /// <summary>
        /// 获取满足条件的实体数量
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="predicate">过滤条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<int> CountAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync(cancellationToken);
        }

        #endregion

        #region 增删改操作

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">实体实例（不能为空）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">实体集合（不能为空且不能包含 null）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) throw new ArgumentException("实体集合不能为空", nameof(entities));

            var entityList = entities.ToList();
            await _context.Set<TEntity>().AddRangeAsync(entityList, cancellationToken);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">被修改后的实体实例</param>
        /// <returns></returns>
        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = _context.Set<TEntity>().Update(entity);
            return entry.Entity;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">被修改后的实体实例集合</param>
        /// <returns></returns>
        public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            var entityList = entities.ToList();
            _context.Set<TEntity>().UpdateRange(entityList);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">删除对象实体</param>
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="id">主键id</param>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task DeleteByIdAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TKey>
        {
            var entity = await GetByIdAsync<TEntity, TKey>(id, cancellationToken);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">批量删除实体模型</param>
        public void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}
