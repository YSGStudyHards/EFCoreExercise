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

            // ע�����ݿ�������
            builder.Services.AddDbContext<SchoolDbContext>();

            //�Զ������ݿ���ʼ����
            //SchoolDbInitializer.Initialize();

            builder.Services.AddControllers();

            // ע�� MiniProfiler ���ķ���
            builder.Services.AddMiniProfiler(options =>
            {
                // ���ʵ�ַ·�ɸ�Ŀ¼��Ĭ��Ϊ��/mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // Sql��ʽ������
                options.SqlFormatter = new InlineFormatter();

                // �������Ӵ򿪹ر�
                options.TrackConnectionOpenClose = true;

                // ����������ɫ����
                options.ColorScheme = ColorScheme.Auto;

                // ����ͼ���з���
                options.EnableMvcViewProfiling = true;

                // �����ʾλ�ã�Ĭ����ࣩ
                options.PopupRenderPosition = RenderPosition.Left;
            }).
            AddEntityFramework();// �� Entity Framework Core �������ܷ���

            // ���Swagger����
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebAPI",
                    Version = "V1",
                    Description = ".NET EF Core��������ʵս�̳�",
                    Contact = new OpenApiContact
                    {
                        Name = "GitHubԴ���ַ",
                        Url = new Uri("https://github.com/YSGStudyHards/DotNetGuide")
                    }
                });

                // ��ȡxml�ļ���
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
                options.IncludeXmlComments(xmlPath, true);
                // ��action�����ƽ�����������ж�����Ϳ��Կ���Ч����
                options.OrderActionsBy(o => o.RelativePath);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    // �� Swagger UI ��ҳ���ó������Զ���� index.html ҳ�棬ע������ַ�����ƴ�ӹ����ǣ�������.index.html
                    c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("WebAPI.index.html");

                    // ���ý����ʱ�Զ��۵�
                    c.DocExpansion(DocExpansion.None);
                });
            }

            // ʹ���Զ����쳣�м�������ڹܵ�����Բ��������쳣��
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiniProfiler();

            app.MapControllers();

            app.Run();
        }
    }
}
