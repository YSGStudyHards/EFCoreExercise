using Entity.DBModel;
using Utility;

namespace Service
{
    public class TestDataCreate
    {
        /// <summary>
        /// 数据库测试数据生成
        /// </summary>
        public static void GenerateDBTestData()
        {
            using (var context = new SchoolDbContext())
            {
                // 插入教师数据
                var teachers = new List<TeacherInfo>
                {
                    new TeacherInfo { TeacherName = "大姚老师", Gender=1, Birthday=Convert.ToDateTime("2000-10-25"), Email ="4566565789@qq.com", Phone="8787845456465", CreateTime = DateTime.Now, UpdateTime= DateTime.Now },
                    new TeacherInfo { TeacherName = "小袁老师", Gender=2, Birthday=Convert.ToDateTime("1997-10-25"), Email ="88888888@qq.com", Phone="87897456456456", CreateTime = DateTime.Now, UpdateTime= DateTime.Now },
                    new TeacherInfo { TeacherName = "时光老师", Gender=1, Birthday=Convert.ToDateTime("1987-10-25"), Email ="99999999@qq.com", Phone="7788992457878", CreateTime = DateTime.Now, UpdateTime= DateTime.Now }
                };
                context.Teachers.AddRange(teachers);
                context.SaveChanges();

                // 插入班级数据，并分配教师
                var classes = new List<ClassInfo>();
                foreach (var teacher in teachers)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        var grade = new Random().Next(1, 6);

                        var classData = new ClassInfo
                        {
                            ClassName = $"{teacher.TeacherName} - {grade}年级 - {i}班",
                            Grade = grade,
                            TeacherID = teacher.TeacherID,
                            CreateTime = DateTime.Now
                        };
                        classes.Add(classData);
                    }
                }
                context.Classes.AddRange(classes);
                context.SaveChanges();

                // 插入学生数据，并分配班级
                var students = new List<StudentInfo>();
                Random random = new Random();
                foreach (var classItem in classes)
                {
                    for (int i = 1; i <= 50; i++)
                    {
                        var student = new StudentInfo
                        {
                            StudentName = ChineseNameGenerator.Generate(),
                            Gender = random.Next(1, 3),
                            Birthday = DateTime.Now.AddYears(-(18 + random.Next(0, 4))),
                            ClassID = classItem.ClassID,
                            ParentPhone = $"1511647558{i}",
                            Address = "广东省深圳市",
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now
                        };
                        students.Add(student);
                    }
                }
                context.Students.AddRange(students);
                context.SaveChanges();
            }
        }
    }
}
