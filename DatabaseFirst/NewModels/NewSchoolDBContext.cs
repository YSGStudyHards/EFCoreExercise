using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DatabaseFirst.NewModels;

public partial class NewSchoolDBContext : DbContext
{
    public NewSchoolDBContext()
    {
    }

    public NewSchoolDBContext(DbContextOptions<NewSchoolDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<classinfo> classinfos { get; set; }

    public virtual DbSet<studentinfo> studentinfos { get; set; }

    public virtual DbSet<teacherinfo> teacherinfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=SchoolDB;user=root;password=ysg123456789.++", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.4-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<classinfo>(entity =>
        {
            entity.HasKey(e => e.ClassID).HasName("PRIMARY");

            entity.Property(e => e.ClassID).HasComment("班级ID");
            entity.Property(e => e.ClassName).HasComment("班级名称");
            entity.Property(e => e.CreateTime).HasComment("创建时间");
            entity.Property(e => e.Grade).HasComment("年级（如：1表示一年级、2表示二年级...）");
            entity.Property(e => e.TeacherID).HasComment("班主任ID（关联TeacherInfo表）");
            entity.Property(e => e.UpdateTime).HasComment("更新时间");
        });

        modelBuilder.Entity<studentinfo>(entity =>
        {
            entity.HasKey(e => e.StudentID).HasName("PRIMARY");

            entity.Property(e => e.StudentID).HasComment("学生ID");
            entity.Property(e => e.Address).HasComment("家庭住址");
            entity.Property(e => e.Birthday).HasComment("学生生日");
            entity.Property(e => e.ClassID).HasComment("所属班级ID（关联ClassInfo表）");
            entity.Property(e => e.CreateTime).HasComment("创建时间");
            entity.Property(e => e.Gender).HasComment("学生性别（1男生，2女生）");
            entity.Property(e => e.ParentPhone).HasComment("家长联系电话");
            entity.Property(e => e.StudentName).HasComment("学生姓名");
            entity.Property(e => e.UpdateTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("更新时间");
        });

        modelBuilder.Entity<teacherinfo>(entity =>
        {
            entity.HasKey(e => e.TeacherID).HasName("PRIMARY");

            entity.Property(e => e.TeacherID).HasComment("老师ID");
            entity.Property(e => e.Birthday).HasComment("老师生日");
            entity.Property(e => e.CreateTime).HasComment("创建时间");
            entity.Property(e => e.Email).HasComment("老师联系邮箱");
            entity.Property(e => e.Gender).HasComment("老师性别（1男生，2女生）");
            entity.Property(e => e.Phone).HasComment("老师联系手机");
            entity.Property(e => e.TeacherName).HasComment("老师名称");
            entity.Property(e => e.UpdateTime).HasComment("更新时间");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
