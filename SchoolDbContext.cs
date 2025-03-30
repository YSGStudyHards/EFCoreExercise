using EFCoreExercise.DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;

namespace EFCoreExercise
{
    public class SchoolDbContext : DbContext
    {
        public DbSet<ClassInfo> Classes { get; set; }
        public DbSet<TeacherInfo> Teachers { get; set; }
        public DbSet<StudentInfo> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;database=SchoolDB;user=root;password=ysg123456789.++";

#if DEBUG
            optionsBuilder
                //.UseLazyLoadingProxies()//启用延迟加载代理
                .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            ).
            LogTo(Console.WriteLine, LogLevel.Information);//SQL语句、参数和执行时间输出到控制台
#else
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
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
        }
    }
}
