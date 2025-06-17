using Entity.DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Service
{
    public class SchoolDbContext : DbContext
    {
        public DbSet<ClassInfo> Classes { get; set; }
        public DbSet<TeacherInfo> Teachers { get; set; }
        public DbSet<StudentInfo> Students { get; set; }

        public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            // 控制台日志
            builder.AddConsole().SetMinimumLevel(LogLevel.Warning); // 全局过滤掉 Information 及以下级别

            // 添加调试窗口日志（需要安装：Microsoft.Extensions.Logging.Debug 包）
            builder.AddDebug();

            // 添加文件日志（注意：需要安装 Serilog、Serilog.Extensions.Hosting、Serilog.Sinks.File 包）
            builder.AddSerilog(new LoggerConfiguration()
                .WriteTo.File("Logs/EF_Core.log")
                .CreateLogger());
        });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;database=SchoolDBNew;user=root;password=ysg123456789.++";

#if DEBUG
            optionsBuilder
                //.UseLazyLoadingProxies()//启用延迟加载代理
                .UseLoggerFactory(_loggerFactory) // 使用日志工厂
                .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                options =>
                {
                    options.CommandTimeout(60);
                } // 设置全局命令超时 60 秒
            ).EnableSensitiveDataLogging(true).EnableDetailedErrors();
            //LogTo(Console.WriteLine, LogLevel.Information);//SQL语句、参数和执行时间输出到控制台
#else
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                options => options.CommandTimeout(60) // 设置全局命令超时 60 秒
            );
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassInfo>()
                .HasMany(b => b.Students) // 配置ClassInfo实体有一个名为Students的导航属性，表示一个班级可以有多个学生
                .WithOne(p => p.ClassInfo) // 配置StudentInfo实体有一个名为ClassInfo的导航属性，这是班级和学生之间关系的反向链接
                .HasForeignKey(s => s.ClassID)  // 显式指定StudentInfo实体中的ClassID属性作为外键，用于关联到ClassInfo实体的主键
                .OnDelete(DeleteBehavior.NoAction); // 显式指定StudentInfo实体中的ClassID属性作为外键，用于关联到ClassInfo实体的主键

            // 配置教师数据
            modelBuilder.Entity<TeacherInfo>().HasData(
                new TeacherInfo
                {
                    TeacherID = 101,
                    TeacherName = "张老师",
                    Gender = 1,
                    Birthday = new DateTime(1980, 5, 10),
                    Phone = "13800138001",
                    Email = "zhang@school.com",
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                },
                new TeacherInfo
                {
                    TeacherID = 201,
                    TeacherName = "李老师",
                    Gender = 2,
                    Birthday = new DateTime(1985, 8, 15),
                    Phone = "13800138002",
                    Email = "li@school.com",
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                });

            // 配置班级数据
            modelBuilder.Entity<ClassInfo>().HasData(
                new ClassInfo
                {
                    ClassID = 1101,
                    ClassName = "一年级一班",
                    Grade = 1,
                    TeacherID = 101,
                    ClassDescription = "1-1",
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                },
                new ClassInfo
                {
                    ClassID = 1102,
                    ClassName = "二年级一班",
                    Grade = 2,
                    TeacherID = 201,
                    ClassDescription = "2-1",
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                });

            // 配置学生数据
            modelBuilder.Entity<StudentInfo>().HasData(
                new StudentInfo
                {
                    StudentID = 1008,
                    StudentName = "张三",
                    Gender = 1,
                    Birthday = new DateTime(2015, 3, 12),
                    ClassID = 1101,
                    ParentPhone = "13800138011",
                    Address = "北京市海淀区",
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                },
                new StudentInfo
                {
                    StudentID = 1002,
                    StudentName = "李四",
                    Gender = 1,
                    Birthday = new DateTime(2015, 5, 18),
                    ClassID = 1102,
                    ParentPhone = "13800138012",
                    Address = "北京市朝阳区",
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                },
                new StudentInfo
                {
                    StudentID = 1003,
                    StudentName = "王小红",
                    Gender = 2,
                    Birthday = new DateTime(2014, 7, 22),
                    ClassID = 1102,
                    ParentPhone = "13800138013",
                    Address = "北京市西城区",
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                });
        }
    }
}
