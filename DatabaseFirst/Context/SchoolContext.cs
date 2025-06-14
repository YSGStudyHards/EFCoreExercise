using System;
using System.Collections.Generic;
using DatabaseFirst.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace DatabaseFirst.Context;

public partial class SchoolContext : DbContext
{
    public SchoolContext()
    {
    }

    public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Classinfo> Classinfos { get; set; }

    public virtual DbSet<Studentinfo> Studentinfos { get; set; }

    public virtual DbSet<Teacherinfo> Teacherinfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=SchoolDB;user=root;password=ysg123456789.++", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.4-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Classinfo>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PRIMARY");

            entity.ToTable("classinfo");

            entity.Property(e => e.ClassId)
                .HasComment("班级ID")
                .HasColumnName("ClassID");
            entity.Property(e => e.ClassName)
                .HasMaxLength(100)
                .HasComment("班级名称");
            entity.Property(e => e.CreateTime)
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Grade).HasComment("年级（如：1表示一年级、2表示二年级...）");
            entity.Property(e => e.TeacherId)
                .HasComment("班主任ID（关联TeacherInfo表）")
                .HasColumnName("TeacherID");
            entity.Property(e => e.UpdateTime)
                .HasComment("更新时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Studentinfo>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PRIMARY");

            entity.ToTable("studentinfo");

            entity.HasIndex(e => e.ClassId, "ClassID");

            entity.Property(e => e.StudentId)
                .HasComment("学生ID")
                .HasColumnName("StudentID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasComment("家庭住址");
            entity.Property(e => e.Birthday)
                .HasComment("学生生日")
                .HasColumnType("datetime");
            entity.Property(e => e.ClassId)
                .HasComment("所属班级ID（关联ClassInfo表）")
                .HasColumnName("ClassID");
            entity.Property(e => e.CreateTime)
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Gender).HasComment("学生性别（1男生，2女生）");
            entity.Property(e => e.ParentPhone)
                .HasMaxLength(50)
                .HasComment("家长联系电话");
            entity.Property(e => e.StudentName)
                .HasMaxLength(80)
                .HasComment("学生姓名");
            entity.Property(e => e.UpdateTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("更新时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Teacherinfo>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PRIMARY");

            entity.ToTable("teacherinfo");

            entity.Property(e => e.TeacherId)
                .HasComment("老师ID")
                .HasColumnName("TeacherID");
            entity.Property(e => e.Birthday)
                .HasComment("老师生日")
                .HasColumnType("datetime");
            entity.Property(e => e.CreateTime)
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasComment("老师联系邮箱");
            entity.Property(e => e.Gender).HasComment("老师性别（1男生，2女生）");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasComment("老师联系手机");
            entity.Property(e => e.TeacherName)
                .HasMaxLength(80)
                .HasComment("老师名称");
            entity.Property(e => e.UpdateTime)
                .HasComment("更新时间")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
