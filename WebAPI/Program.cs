using Entity;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Service;
using StackExchange.Profiling;
using StackExchange.Profiling.SqlFormatters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using WebAPI.Middleware;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // 注册数据库上下文
            builder.Services.AddDbContext<SchoolDbContext>();

            //自定义数据库表初始化器
            //SchoolDbInitializer.Initialize();

            builder.Services.AddControllers();

            // 注册 MiniProfiler 核心服务
            builder.Services.AddMiniProfiler(options =>
            {
                // 访问地址路由根目录，默认为：/mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // Sql格式化设置
                options.SqlFormatter = new InlineFormatter();

                // 跟踪连接打开关闭
                options.TrackConnectionOpenClose = true;

                // 界面主题颜色方案
                options.ColorScheme = ColorScheme.Auto;

                // 对视图进行分析
                options.EnableMvcViewProfiling = true;

                // 面板显示位置（默认左侧）
                options.PopupRenderPosition = RenderPosition.Left;
            }).
            AddEntityFramework();// 对 Entity Framework Core 进行性能分析

            // 添加Swagger服务
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebAPI",
                    Version = "V1",
                    Description = ".NET EF Core快速入门实战教程",
                    Contact = new OpenApiContact
                    {
                        Name = "GitHub源码地址",
                        Url = new Uri("https://github.com/YSGStudyHards/DotNetGuide")
                    }
                });

                // 获取xml文件名
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // 获取xml文件路径
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 添加控制器层注释，true表示显示控制器注释
                options.IncludeXmlComments(xmlPath, true);
                // 对action的名称进行排序，如果有多个，就可以看见效果了
                options.OrderActionsBy(o => o.RelativePath);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    // 将 Swagger UI 首页设置成我们自定义的 index.html 页面，注意这个字符串的拼接规则是：程序集名.index.html
                    c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("WebAPI.index.html");

                    // 设置界面打开时自动折叠
                    c.DocExpansion(DocExpansion.None);
                });
            }

            // 使用自定义异常中间件（放在管道最顶部以捕获所有异常）
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiniProfiler();

            app.MapControllers();

            app.Run();
        }
    }
}
