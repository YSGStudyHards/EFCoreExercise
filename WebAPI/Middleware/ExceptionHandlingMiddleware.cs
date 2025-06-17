using Entity;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Middleware
{
    /// <summary>
    /// 自定义全局异常处理中间件
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// 依赖注入全局异常处理中间件
        /// </summary>
        /// <param name="next">next</param>
        /// <param name="logger">logger</param>
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problem = new ApiResponse<int>
            {
                Success = false,
                Message = "程序异常"
            };

            // 异常分类处理
            switch (exception)
            {
                case DbUpdateConcurrencyException ex:
                    problem.Message = $"数据并发冲突 {ex.Message}";
                    _logger.LogWarning(ex, "并发冲突: {TraceId}", context.TraceIdentifier);
                    break;
                case DbUpdateException ex:
                    problem.Message = $"数据库更新异常 {ex.Message}";
                    _logger.LogError(ex, "数据库更新失败: {TraceId}", context.TraceIdentifier);
                    break;
                case TimeoutException:
                    problem.Message = $"数据库操作执行超时";
                    _logger.LogWarning("请求超时: {TraceId}", context.TraceIdentifier);
                    break;
                case UnauthorizedAccessException:
                    problem.Message = $"您没有权限执行此操作";
                    _logger.LogWarning("未授权访问: {Path}", context.Request.Path);
                    break;
                default:
                    problem.Message = $"处理请求时发生意外错误";
                    _logger.LogError(exception, "未处理异常: {TraceId}", context.TraceIdentifier);
                    break;
            }

            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(problem);
        }
    }
}
