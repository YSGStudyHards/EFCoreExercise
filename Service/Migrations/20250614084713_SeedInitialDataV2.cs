using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Service.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialDataV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TeacherInfo",
                columns: new[] { "TeacherID", "Birthday", "CreateTime", "Email", "Gender", "Phone", "TeacherName", "UpdateTime" },
                values: new object[,]
                {
                    { 101, new DateTime(1980, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(8916), "zhang@school.com", 1, "13800138001", "张老师", new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(8907) },
                    { 201, new DateTime(1985, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(8919), "li@school.com", 2, "13800138002", "李老师", new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(8918) }
                });

            migrationBuilder.InsertData(
                table: "ClassInfo",
                columns: new[] { "ClassID", "ClassDescription", "ClassName", "CreateTime", "Grade", "TeacherID", "UpdateTime" },
                values: new object[,]
                {
                    { 1101, "1-1", "一年级一班", new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9024), 1, 101, new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9023) },
                    { 1102, "2-1", "二年级一班", new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9026), 2, 201, new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9026) }
                });

            migrationBuilder.InsertData(
                table: "StudentInfo",
                columns: new[] { "StudentID", "Address", "Birthday", "ClassID", "CreateTime", "Gender", "ParentPhone", "StudentName", "UpdateTime" },
                values: new object[,]
                {
                    { 1001, "北京市海淀区", new DateTime(2015, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1101, new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9049), 1, "13800138011", "张三", new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9048) },
                    { 1002, "北京市朝阳区", new DateTime(2015, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1102, new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9051), 1, "13800138012", "李四", new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9051) },
                    { 1003, "北京市西城区", new DateTime(2014, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1102, new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9053), 2, "13800138013", "王小红", new DateTime(2025, 6, 14, 16, 47, 13, 189, DateTimeKind.Local).AddTicks(9053) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StudentInfo",
                keyColumn: "StudentID",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "StudentInfo",
                keyColumn: "StudentID",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "StudentInfo",
                keyColumn: "StudentID",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "ClassInfo",
                keyColumn: "ClassID",
                keyValue: 1101);

            migrationBuilder.DeleteData(
                table: "ClassInfo",
                keyColumn: "ClassID",
                keyValue: 1102);

            migrationBuilder.DeleteData(
                table: "TeacherInfo",
                keyColumn: "TeacherID",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "TeacherInfo",
                keyColumn: "TeacherID",
                keyValue: 201);
        }
    }
}
