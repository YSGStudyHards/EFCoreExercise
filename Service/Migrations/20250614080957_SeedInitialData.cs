using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <summary>
        /// 定义如何将数据库从当前状态升级到新状态
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 插入教师数据
            migrationBuilder.InsertData(
                table: "TeacherInfo",
                columns: new[] { "TeacherID", "TeacherName", "Gender", "Birthday", "Phone", "Email", "CreateTime" },
                values: new object[,]
                {
                    { 1, "张老师", 1, new DateTime(1980, 5, 10), "13800138001", "zhang@school.com", DateTime.Now },
                    { 2, "李老师", 2, new DateTime(1985, 8, 15), "13800138002", "li@school.com", DateTime.Now }
                });

            // 插入班级数据
            migrationBuilder.InsertData(
                table: "ClassInfo",
                columns: new[] { "ClassID", "ClassName", "Grade", "TeacherID", "CreateTime", "ClassDescription" },
                values: new object[,]
                {
                    { 101, "一年级一班", 1, 1, DateTime.Now,"1-1班" },
                    { 102, "二年级一班", 2, 2, DateTime.Now , "2-1班"}
                });

            // 插入学生数据
            migrationBuilder.InsertData(
                table: "StudentInfo",
                columns: new[] { "StudentID", "StudentName", "Gender", "Birthday", "ClassID", "ParentPhone", "Address", "CreateTime" },
                values: new object[,]
                {
                    { 1001, "张三", 1, new DateTime(2015, 3, 12), 101, "13800138011", "北京市海淀区", DateTime.Now },
                    { 1002, "李四", 1, new DateTime(2015, 5, 18), 101, "13800138012", "北京市朝阳区", DateTime.Now },
                    { 1003, "王小红", 2, new DateTime(2014, 7, 22), 102, "13800138013", "北京市西城区", DateTime.Now }
                });
        }

        /// <summary>
        /// 回滚操作：删除所有插入的数据
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM StudentInfo WHERE StudentID IN (1001, 1002, 1003)");
            migrationBuilder.Sql("DELETE FROM ClassInfo WHERE ClassID IN (101, 102)");
            migrationBuilder.Sql("DELETE FROM TeacherInfo WHERE TeacherID IN (1, 2)");
        }
    }
}
