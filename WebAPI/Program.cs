using Microsoft.OpenApi.Models;
using Service;
using System.Reflection;

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

            builder.Services.AddControllers();

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
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
