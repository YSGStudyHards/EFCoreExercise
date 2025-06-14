using Entity.DBModel;

namespace Service
{
    /// <summary>
    /// 自定义数据库表初始化器
    /// </summary>
    public static class SchoolDbInitializer
    {
        public static void Initialize()
        {
            using var context = new SchoolDbContext();

            // 确保数据库已创建
            context.Database.EnsureCreated();

            // 检查是否已有数据
            if (context.Teachers.Any())
            {
                return; // 数据库已初始化
            }

            // 添加教师
            var teachers = new TeacherInfo[]
            {
                new TeacherInfo
                {
                    TeacherName = "张老师",
                    Gender = 1,
                    Birthday = new DateTime(1980, 5, 10),
                    Phone = "13800138001",
                    Email = "zhang@school.com",
                    CreateTime = DateTime.Now
                },
                new TeacherInfo
                {
                    TeacherName = "李老师",
                    Gender = 2,
                    Birthday = new DateTime(1985, 8, 15),
                    Phone = "13800138002",
                    Email = "li@school.com",
                    CreateTime = DateTime.Now
                }
            };
            context.Teachers.AddRange(teachers);
            context.SaveChanges();

            // 添加班级
            var classes = new ClassInfo[]
            {
                new ClassInfo
                {
                    ClassName = "一年级一班",
                    Grade = 1,
                    TeacherID = teachers[0].TeacherID,
                    ClassDescription="1-1",
                    CreateTime = DateTime.Now
                },
                new ClassInfo
                {
                    ClassName = "二年级一班",
                    Grade = 2,
                    TeacherID = teachers[1].TeacherID,
                    ClassDescription="2-1",
                    CreateTime = DateTime.Now
                }
            };
            context.Classes.AddRange(classes);
            context.SaveChanges();

            // 添加学生
            var students = new StudentInfo[]
            {
                new StudentInfo
                {
                    StudentName = "张三",
                    Gender = 1,
                    Birthday = new DateTime(2015, 3, 12),
                    ClassID = classes[0].ClassID,
                    ParentPhone = "13800138011",
                    Address = "北京市海淀区",
                    CreateTime = DateTime.Now
                },
                new StudentInfo
                {
                    StudentName = "李四",
                    Gender = 1,
                    Birthday = new DateTime(2015, 5, 18),
                    ClassID = classes[0].ClassID,
                    ParentPhone = "13800138012",
                    Address = "北京市朝阳区",
                    CreateTime = DateTime.Now
                },
                new StudentInfo
                {
                    StudentName = "王小红",
                    Gender = 2,
                    Birthday = new DateTime(2014, 7, 22),
                    ClassID = classes[1].ClassID,
                    ParentPhone = "13800138013",
                    Address = "北京市西城区",
                    CreateTime = DateTime.Now
                }
            };
            context.Students.AddRange(students);
            context.SaveChanges();
        }
    }
}
