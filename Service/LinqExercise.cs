﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    /// <summary>
    /// LINQ中常用方法
    /// </summary>
    public class LinqExercise
    {
        #region LINQ中常用方法

        public class StudentInfo
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public DateTime Birthday { get; set; }
            public int ClassID { get; set; }
            public string Address { get; set; }
            public List<Course> Courses { get; set; } = new List<Course>();
        }

        public class Course
        {
            public int CourseID { get; set; }
            public string CourseName { get; set; }
        }

        static List<Course> courses = new List<Course>()
        {
          new Course
          {
            CourseID =  101,
            CourseName = "语文"
          },
          new Course
          {
            CourseID =  102,
            CourseName = "数学"
          },
          new Course
          {
            CourseID =  103,
            CourseName = "地理"
          },
          new Course
          {
            CourseID =  104,
            CourseName = "历史"
          }
        };

        static List<StudentInfo> students = new List<StudentInfo>
        {
            new StudentInfo
            {
                StudentID=1,
                StudentName="大姚",
                Birthday=Convert.ToDateTime("1997-10-25"),
                ClassID=101,
                Courses = new List<Course>
                {
                    new Course { CourseID = 101, CourseName = "语文" },
                    new Course { CourseID = 102, CourseName = "数学" }
                }
            },
            new StudentInfo
            {
                StudentID=2,
                StudentName="李四",
                Birthday=Convert.ToDateTime("1998-10-25"),
                ClassID=101,
                Courses = new List<Course>
                {
                    new Course { CourseID = 101, CourseName = "语文" },
                    new Course { CourseID = 102, CourseName = "数学" }
                }
            },
            new StudentInfo
            {
                StudentID=3,
                StudentName="王五",
                Birthday=Convert.ToDateTime("1999-10-25"),
                ClassID=102,
                Address="广州",
                Courses = new List<Course>
                {
                    new Course { CourseID = 101, CourseName = "语文" },
                    new Course { CourseID = 102, CourseName = "数学" }
                }
            },
            new StudentInfo
            {
                StudentID=4,
                StudentName="时光者",
                Birthday=Convert.ToDateTime("1999-11-25"),
                ClassID=102,
                Address="深圳" ,
                Courses = new List<Course>
                {
                    new Course { CourseID = 104, CourseName = "历史" },
                    new Course { CourseID = 103, CourseName = "地理" }
                }
            }
        };

        public static void CommonMethodsInLINQ()
        {
            #region 基本查询方法

            var femaleStudents = students.Where(s => s.StudentName == "追逐时光者");
            var studentNames = students.Select(s => s.StudentName);

            // 使用SelectMany展平所有学生的课程列表
            // SelectMany用于将多个集合（嵌套集合，如集合的集合）`展平`为一个集合。
            var allCourses = students.SelectMany(student => student.Courses).ToList();

            // 输出所有课程的名称
            foreach (var course in allCourses)
            {
                Console.WriteLine(course.CourseName);
            }

            #endregion

            #region 转换方法

            var studentList = students.ToList();
            var studentArray = students.ToArray();
            var studentDictionary = students.ToDictionary(s => s.StudentID, s => s.StudentName);
            var studentLookup = students.ToLookup(s => s.ClassID, s => s.StudentName);

            #endregion

            #region 元素操作方法

            var firstStudent = students.First();
            var firstAdult = students.FirstOrDefault(s => s.Birthday <= DateTime.Now.AddYears(-18));
            var onlyWangWu = students.Single(s => s.StudentName == "王五");
            var wangWuOrDefault = students.SingleOrDefault(s => s.StudentName == "王六");
            var lastStudent = students.Last();
            var lastAdult = students.LastOrDefault(s => s.Birthday <= DateTime.Now.AddYears(-18));
            var secondStudent = students.ElementAt(1);
            var tenthStudentOrDefault = students.ElementAtOrDefault(9);
            var nonEmptyStudents = students.DefaultIfEmpty(new StudentInfo { StudentID = 0, StudentName = "默认Student", Address = "默认" });

            #endregion

            #region 排序方法

            var sortedByBirthdayAsc = students.OrderBy(s => s.Birthday);
            var sortedByClassIDDesc = students.OrderByDescending(s => s.ClassID);
            var sortedByNameThenClassID = students.OrderBy(s => s.StudentName).ThenBy(s => s.ClassID);
            var sortedThenByDescending = students.OrderBy(s => s.StudentName).ThenBy(s => s.ClassID).ThenByDescending(x => x.Birthday);

            #endregion

            #region 聚合方法

            int studentCount = students.Count();
            int totalClassID = students.Sum(s => s.ClassID);
            double averageAge = students.Average(s => DateTime.Now.Year - s.Birthday.Year);
            int minClassID = students.Min(s => s.ClassID);
            int maxClassID = students.Max(s => s.ClassID);
            string concatenatedNames = students.Aggregate("", (acc, s) => acc == "" ? s.StudentName : acc + ", " + s.StudentName);

            #endregion

            #region 集合操作方法

            var uniqueClassIDs = students.Select(s => s.ClassID).Distinct();//
            var unionClassIDs = uniqueClassIDs.Union(new[] { 103, 104 });
            var intersectClassIDs = uniqueClassIDs.Intersect(new[] { 101, 103 });
            var exceptClassIDs = uniqueClassIDs.Except(new[] { 101 });
            var concatClassIDs = uniqueClassIDs.Concat(new[] { 103, 104 });

            #endregion

            #region 分组与连接方法

            var groupedByClassID = students.GroupBy(s => s.ClassID);

            foreach (var group in groupedByClassID)
            {
                Console.WriteLine($"班级ID: {group.Key}");
                foreach (var student in group)
                {
                    Console.WriteLine($"  学生姓名: {student.StudentName}");
                }
            }

            // 连接两个集合（内连接查询）
            var otherStudent = new List<StudentInfo>
            {
               new StudentInfo
               {
                   StudentID=4,
                   StudentName="摇一摇",
                   Birthday=Convert.ToDateTime("1997-10-25"),
                   ClassID=101,
                   Courses = new List<Course>
                   {
                       new Course { CourseID = 101, CourseName = "语文" },
                       new Course { CourseID = 102, CourseName = "数学" }
                   }
               }
            };

            var listJoin = students.Join(
                otherStudent, // 要连接的第二个集合
                s1 => s1.StudentID, // 从第一个集合中提取键
                s2 => s2.StudentID, // 从第二个集合中提取键
                (s1, s2) => new // 结果选择器，指定如何从两个匹配元素创建结果
                {
                    StudentID = s1.StudentID,
                    StudentName = s1.StudentName,
                    Birthday = s1.Birthday,
                    ClassID = s1.ClassID,
                    Address = s1.Address,
                    Courses = s1.Courses,
                    OtherStudentName = s2.StudentName
                });

            //使用 GroupJoin 方法实现两个集合的左连接（Left Join）
            //目标：获取所有课程及选修学生（即使无人选修也要显示课程）
            var courseStudentGroups = courses.GroupJoin(
                students.SelectMany(
                    student => student.Courses,
                    (student, course) => new { Student = student, Course = course }
                ),
                course => course.CourseID,
                studentCoursePair => studentCoursePair.Course.CourseID,
                // 结果投影：生成课程名称及对应的学生列表
                (course, matchedStudents) => new
                {
                    CourseName = course.CourseName,
                    Students = matchedStudents
                        .Select(pair => pair.Student.StudentName)
                        .DefaultIfEmpty("（无学生）")
                        .ToList()
                }
            ).ToList();

            // 输出结果
            foreach (var group in courseStudentGroups)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine($"课程：{group.CourseName}");
                Console.WriteLine($"选修学生：{string.Join(", ", group.Students)}");
                Console.WriteLine("-------------------");
            }

            #endregion

            #region 跳过与获取指定数量的元素

            var skippedStudents = students.Skip(1);
            var takenStudents = students.Take(2);

            //数据分页查询（Skip + Take）
            int pageNumber = 2;
            int pageSize = 10;
            var pagedUsers = skippedStudents
                .OrderBy(u => u.ClassID) // 必须排序
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            #endregion

            #region 条件判断方法

            bool allAdults = students.All(s => s.Birthday <= DateTime.Now.AddYears(-18));
            bool anyAdults = students.Any(s => s.Birthday <= DateTime.Now.AddYears(-18));
            bool containsWangWu = students.Contains(students.First(s => s.StudentName == "王五"));

            #endregion

            #region 使用查询语法

            var querySyntaxResult = from student in students
                                    where student.ClassID == 101
                                    orderby student.StudentName ascending
                                    select student;

            Console.WriteLine("查询语法结果:");
            foreach (var student in querySyntaxResult)
            {
                Console.WriteLine($"{student.StudentName}, ClassID: {student.ClassID}");
            }

            #endregion

            #region 使用方法语法

            var methodSyntaxResult = students
                                    .Where(student => student.ClassID == 101)
                                    .OrderBy(student => student.StudentName)
                                    .ToList();


            Console.WriteLine("方法语法结果:");
            foreach (var student in methodSyntaxResult)
            {
                Console.WriteLine($"{student.StudentName}, ClassID: {student.ClassID}");
            }

            #endregion

            #region 混合查询和方法语法

            var mixedResult = (from student in students
                               where student.ClassID == 101
                               where student.Courses.Any(course => course.CourseName == "数学")
                               orderby student.StudentName ascending
                               select student)
                       .Take(2)
                       .ToList();

            // 输出结果
            Console.WriteLine("混合查询结果:");
            foreach (var student in mixedResult)
            {
                Console.WriteLine($"{student.StudentName}, ClassID: {student.ClassID}");
            }

            #endregion
        }

        #endregion
    }
}
