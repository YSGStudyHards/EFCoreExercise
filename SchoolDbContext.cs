using EFCoreExercise.DBModel;
using Microsoft.EntityFrameworkCore;

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
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            );
        }
    }
}
