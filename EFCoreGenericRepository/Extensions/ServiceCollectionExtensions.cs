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
        /// 添加通用仓储服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddGenericRepository(this IServiceCollection services)
        {
            services.AddScoped<IQueryRepository, Repository>();
            services.AddScoped<IRepository, Repository>();
            return services;
        }

        /// <summary>
        /// 添加通用仓储服务（指定DbContext）
        /// </summary>
        /// <typeparam name="TContext">DbContext类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddGenericRepository<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IQueryRepository>(provider =>
                new Repository(provider.GetRequiredService<TContext>()));
            services.AddScoped<IRepository>(provider =>
                new Repository(provider.GetRequiredService<TContext>()));
            return services;
        }
    }
}
