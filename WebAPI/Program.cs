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

            // ע�����ݿ�������
            builder.Services.AddDbContext<SchoolDbContext>();

            builder.Services.AddControllers();

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
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
