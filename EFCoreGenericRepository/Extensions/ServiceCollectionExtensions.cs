using EFCoreGenericRepository.Implementations;
using EFCoreGenericRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EFCoreGenericRepository.Extensions
{
    /// <summary>
    /// 服务集合扩展方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加通用仓储服务（指定DbContext）
        /// 仅注册仓储（自动保存模式）
        /// </summary>
        /// <typeparam name="TDbContext">DbContext类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddGenericRepository<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            //注册数据库上下文
            services.AddScoped<TDbContext>();

            //注册通用仓储
            services.AddScoped<IRepository<TDbContext>, Repository<TDbContext>>();
            services.AddScoped<IQueryRepository<TDbContext>, Repository<TDbContext>>();
            return services;
        }

        /// <summary>
        /// 同时注册仓储 + 工作单元。
        /// IRepository<TDbContext>：自动保存（适合简单场景）
        /// IUnitOfWork<TDbContext>：延迟提交（批量/事务）
        /// </summary>
        public static IServiceCollection AddGenericRepositoryWithUnitOfWork<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            //注册数据库上下文
            services.AddScoped<TDbContext>();

            //注册通用仓储
            services.AddScoped<IRepository<TDbContext>, Repository<TDbContext>>();
            services.AddScoped<IQueryRepository<TDbContext>, Repository<TDbContext>>();

            // 工作单元（内部会创建 autoSaveChanges:false 的仓储实例）
            services.AddScoped<IUnitOfWork<TDbContext>, UnitOfWork<TDbContext>>();

            return services;
        }
    }
}
