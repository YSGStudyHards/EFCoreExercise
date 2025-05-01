using Entity.DBModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    /// <summary>
    /// EF Core数据CRUD简单操作
    /// </summary>
    public class SimpleOperation
    {
        public static void AddData()
        {
            #region 添加数据

            //单条记录添加
            using (var context = new SchoolDbContext())
            {
                var teacher = new TeacherInfo
                {
                    TeacherName = "张老师",
                    Gender = 1,
                    Birthday = Convert.ToDateTime("1996-11-22"),
                    Phone = "13165856687",
                    Email = "qifei@qq.com",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                context.Teachers.Add(teacher);
                context.SaveChanges();
            }

            //多条记录批量添加
            using (var context = new SchoolDbContext())
            {
                var teacherList = new List<TeacherInfo>();
                for (int i = 1; i < 11; i++)
                {
                    teacherList.Add(new TeacherInfo
                    {
                        TeacherName = $"小袁老师{i}号",
                        Gender = 2,
                        Birthday = Convert.ToDateTime("2000-11-22"),
                        Phone = "12345678955245",
                        Email = $"xiaoyuan—{i}@email.com",
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    });
                }

                context.AddRange(teacherList);
                context.SaveChanges();
            }

            #endregion
        }

        public static void DataQueryOperation()
        {
            #region 查询数据

            using (var context = new SchoolDbContext())
            {
                //条件过滤（Where）,查询所有男老师（Gender：1男生，2女生）
                var queryAllMaleTeachers = context.Teachers.Where(t => t.Gender == 1).ToList();

                //排序查询（OrderBy / OrderByDescending）
                var teacherOrderList = context.Teachers
                    .OrderBy(u => u.TeacherName)          // 按姓名升序
                    .ThenByDescending(u => u.TeacherName) // 多字段排序：按编号降序
                    .ToList();

                //数据分页查询（Skip + Take）
                int pageNumber = 2;
                int pageSize = 10;
                var pagedUsers = context.Teachers
                    .OrderBy(u => u.TeacherID) // 必须排序
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 基础投影（选择指定列）
                var teachersBasic = context.Teachers
                    .Select(t => new
                    {
                        t.TeacherID,
                        TeacherName = t.TeacherName, // 字段别名
                        BirthYear = t.Birthday.Year   // 计算字段
                    }).ToList();

                // 分组统计男女教师数量
                var genderStats = context.Teachers
                    .GroupBy(t => t.Gender)
                    .Select(g => new
                    {
                        Gender = g.Key == 1 ? "男老师" : "女老师",
                        Count = g.Count()
                    }).ToList();

                //统计男教师人数
                var maleCount = context.Teachers.Count(t => t.Gender == 1);

                var birthdayMax = context.Teachers.Max(t => t.Birthday);
                var birthdayMix = context.Teachers.Min(t => t.Birthday);
            }

            #endregion
        }

        public static void UpdateData()
        {
            #region 更新数据

            //单条记录的更新
            using (var context = new SchoolDbContext())
            {
                // 根据ID查询要修改的教师
                var teacher = context.Teachers.FirstOrDefault(t => t.TeacherID == 13);
                if (teacher != null)
                {
                    // 修改属性
                    teacher.TeacherName = "小红老师";
                    teacher.Email = "new_email@example.com";
                    teacher.UpdateTime = DateTime.Now;
                    context.SaveChanges();
                }
            }

            //多条记录批量更新
            using (var context = new SchoolDbContext())
            {
                // 更新所有女教师的邮箱后缀
                var ExecuteNum = context.Teachers.Where(t => t.Gender == 2).ExecuteUpdate(setters => setters.SetProperty(t => t.Email, t => t.Email + ".cn").SetProperty(t => t.UpdateTime, DateTime.Now));
            }

            #endregion
        }

        public static void DeleteData()
        {
            #region 删除数据

            //单条记录删除
            using (var context = new SchoolDbContext())
            {
                // 查询要删除的实体
                var teacherToDelete = context.Teachers.FirstOrDefault(t => t.TeacherID == 21);
                if (teacherToDelete != null)
                {
                    context.Teachers.Remove(teacherToDelete);
                    context.SaveChanges();
                }
            }

            //多条记录批量删除
            using (var context = new SchoolDbContext())
            {
                // 删除所有Gender=1的记录
                var deleteNum = context.Teachers
                     .Where(t => t.Gender == 1)
                     .ExecuteDelete();
            }

            #endregion
        }
    }
}
