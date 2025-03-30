using EFCoreExercise.DBModel;
using EFCoreExercise.ViewModel;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EFCoreExercise
{
    /// <summary>
    /// EF Core高级查询技巧与实操
    /// </summary>
    public class AdvancedQuery
    {

        /// <summary>
        /// 多表关联查询
        /// </summary>
        public static void RelationalQueryExample()
        {
            using (var db = new SchoolDbContext())
            {
                #region 内连接查询

                var innerJoinQuery = from s in db.Students
                                     join c in db.Classes on s.ClassID equals c.ClassID
                                     join t in db.Teachers on c.TeacherID equals t.TeacherID
                                     select new
                                     {
                                         s.StudentName,
                                         c.ClassName,
                                         t.TeacherName
                                     };

                #endregion

                #region 左连接查询

                var leftJoinQuery = from c in db.Classes
                                    join t in db.Teachers on c.TeacherID equals t.TeacherID into teacherGroup
                                    from t in teacherGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        c,
                                        t
                                    };

                var leftJoinQuery2 = db.Classes
    // 第一步：GroupJoin 创建分组关联
    .GroupJoin(
        db.Teachers,
        c => c.TeacherID,// 左表关联键（Class 的 TeacherID）
        t => t.TeacherID,// 右表关联键（Teacher 的 TeacherID）
        (c, teacherGroup) => new
        {
            Class = c,
            Teachers = teacherGroup
        }
    )
    // 第二步：SelectMany 展开分组并处理空值
    .SelectMany(
        temp => temp.Teachers.DefaultIfEmpty(),// 确保即使无关联教师也保留 Class 班级信息
        (temp, t) => new
        {
            temp.Class,
            Teacher = t
        }
    );
                #endregion

                #region 右连接查询

                var rightJoinQuery = from t in db.Teachers
                                     join c in db.Classes on t.TeacherID equals c.TeacherID into classGroup
                                     from c in classGroup.DefaultIfEmpty()
                                     select new
                                     {
                                         t,
                                         c
                                     };

                #endregion
            }
        }

        /// <summary>
        /// 复杂条件过滤示例
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="minBirthday">minBirthday</param>
        /// <param name="minAge">minAge</param>
        /// <returns></returns>
        public static async Task<List<StudentInfo>> ComplexFilterExample(string name, DateTime? minBirthday, int? minAge)
        {
            using (var db = new SchoolDbContext())
            {
                IQueryable<StudentInfo> query = db.Students;

                // 动态条件构建
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(s => s.StudentName.Contains(name));
                }

                if (minBirthday.HasValue)
                {
                    query = query.Where(s => s.Birthday >= minBirthday.Value);
                }

                if (minAge.HasValue)
                {
                    var minYear = DateTime.Today.AddYears(-minAge.Value).Year;
                    query = query.Where(s => s.Birthday.Year <= minYear);
                }

                return await query
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        /// <summary>
        /// 排序与分组
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ClassStudentCountDto>> GroupByAndOrderByExample()
        {
            using (var db = new SchoolDbContext())
            {
                return await db.Students
                    .GroupBy(s => s.ClassID)
                    .Select(g => new ClassStudentCountDto
                    {
                        ClassId = g.Key,
                        StudentCount = g.Count(),
                        MaxBirthday = g.Max(s => s.Birthday)
                    })
                    .OrderByDescending(x => x.StudentCount)
                    .ThenByDescending(x => x.ClassId)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        /// <summary>
        /// 动态分页
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="sortField">sortField</param>
        /// <param name="isDescending">isDescending</param>
        /// <returns></returns>
        public static async Task<PagedResult<StudentInfo>> PaginationExample(
            int pageIndex = 1,
            int pageSize = 20,
            string sortField = "StudentID",
            bool isDescending = true)
        {
            using (var db = new SchoolDbContext())
            {
                IQueryable<StudentInfo> query = db.Students;

                // 动态排序
                query = sortField switch
                {
                    "StudentName" => isDescending
                        ? query.OrderByDescending(s => s.StudentName)
                        : query.OrderBy(s => s.StudentName),
                    "Birthday" => isDescending
                        ? query.OrderByDescending(s => s.Birthday)
                        : query.OrderBy(s => s.Birthday),
                    _ => isDescending
                        ? query.OrderByDescending(s => s.StudentID)
                        : query.OrderBy(s => s.StudentID)
                };

                var totalCount = await query.CountAsync();
                var items = await query.OrderBy(s => s.StudentID)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();

                return new PagedResult<StudentInfo>(totalCount, pageIndex, pageSize, items);
            }
        }

        /// <summary>
        /// 导航属性加载关联数据
        /// </summary>
        public static void NavigationPropertyLoading()
        {
            using (var db = new SchoolDbContext())
            {
                // 预先加载
                var classes = db.Classes
                    .Include(s => s.Teacher)
                    .ToList();

                var classInfo = db.Classes.Include(x => x.Students).ToList();
            }

            using (var dbContext = new SchoolDbContext())
            {
                var classInfo = dbContext.Classes.FirstOrDefault();

                if (classInfo != null)
                {
                    // 显式加载ClassInfo的Teacher导航属性
                    dbContext.Entry(classInfo).Reference(c => c.Teacher).Load();

                    Console.WriteLine($"Class: {classInfo.ClassName}, Teacher: {classInfo.Teacher?.TeacherName}");
                }
            }

            using (var dbContext = new SchoolDbContext())
            {
                var classInfo = dbContext.Classes.FirstOrDefault(x => x.ClassID == 12);

                if (classInfo != null)
                {
                    // 延迟加载
                    // 当访问classInfo.Teacher时，EF Core将自动加载Teacher实体
                    Console.WriteLine($"Class: {classInfo.ClassName}, Teacher: {classInfo.Teacher?.TeacherName}");
                }
            }
        }

        /// <summary>
        /// 拆分查询
        /// </summary>
        public static void AsSplitQueryExample()
        {
            using (var db = new SchoolDbContext())
            {
                var classInfo = db.Classes.
                    Include(x => x.Students)
                    .AsSplitQuery()
                    .ToList();
            }
        }

        /// <summary>
        /// 原生SQL查询
        /// </summary>
        public static void NativeSQLQuery()
        {
            using (var db = new SchoolDbContext())
            {
                // 使用FromSql查询学生性别为女生
                var students = db.Students.FromSql($"SELECT * FROM studentinfo WHERE Gender = {2}").ToList();

                // 调用存储过程
                //var studentByClassId = db.Students.FromSql($"EXEC dbo.GetStudentsByClassID @filterByClass={1}").ToList();
            }

            using (var db = new SchoolDbContext())
            {
                var queryName = "%张%";
                var students = db.Students.FromSqlInterpolated($"SELECT * FROM studentinfo WHERE StudentName Like {queryName}").ToList();
            }

            using (var db = new SchoolDbContext())
            {
                // 动态拼接SQL（如动态列名）
                string columnName = "Address";
                var addressParam = new MySqlParameter("@value", "%深圳市%");
                var students = db.Students.FromSqlRaw($"SELECT * FROM studentinfo WHERE {columnName} LIKE @value", addressParam).ToList();
            }

            using (var db = new SchoolDbContext())
            {
                // 使用FromSql查询学生性别为女生，出生日期大于2000年，最后按创建时间降序 
                var students = db.Students.FromSql($"SELECT * FROM studentinfo WHERE Gender = {2}").
                    Where(s => s.Birthday.Year > 2000)
                    .OrderByDescending(s => s.CreateTime).ToList();
            }
        }

    }
}
