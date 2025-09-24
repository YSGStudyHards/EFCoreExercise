using EFCoreGenericRepository.Interfaces;
using Entity.DBModel;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Service
{
    /// <summary>
    /// 演示基于 UnitOfWork 的事务性操作服务
    /// </summary>
    public class TeacherUnitOfWorkService
    {
        private readonly IUnitOfWork<SchoolDbContext> _uow;

        public TeacherUnitOfWorkService(IUnitOfWork<SchoolDbContext> uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// 显式手动 Begin / Commit / Rollback 方式
        /// </summary>
        public async Task<int> CreateTeacherWithStudentsExplicitAsync(
            CreateTeacherRequest request,
            CancellationToken ct = default)
        {
            Validate(request);

            await _uow.BeginTransactionAsync(ct);
            try
            {
                var teacher = new TeacherInfo
                {
                    TeacherName = request.TeacherName,
                    Age = request.Age,
                    CourseName = request.CourseName,
                    CreateTime = DateTime.UtcNow
                };
                await _uow.Repository.AddAsync(teacher, ct);

                if (request.Students != null)
                {
                    foreach (var s in request.Students)
                    {
                        var stu = new StudentInfo
                        {
                            StudentName = s.StudentName,
                            Age = s.Age,
                            ClassName = s.ClassName,
                            CreateTime = DateTime.UtcNow,
                            TeacherId = teacher.Id
                        };
                        await _uow.Repository.AddAsync(stu, ct);
                    }
                }

                // 延迟保存 -> Commit 内部 SaveChanges + Commit
                await _uow.CommitAsync(ct);
                return teacher.Id;
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// 使用 ExecuteInTransactionAsync 包装（更少模板代码）
        /// </summary>
        public async Task<int> CreateTeacherWithStudentsWrappedAsync(
            CreateTeacherRequest request,
            CancellationToken ct = default)
        {
            Validate(request);

            var teacher = new TeacherInfo
            {
                TeacherName = request.TeacherName,
                Age = request.Age,
                CourseName = request.CourseName,
                CreateTime = DateTime.UtcNow
            };

            await _uow.ExecuteInTransactionAsync(async repo =>
            {
                await repo.AddAsync(teacher, ct);

                if (request.Students != null)
                {
                    foreach (var s in request.Students)
                    {
                        await repo.AddAsync(new StudentInfo
                        {
                            StudentName = s.StudentName,
                            Age = s.Age,
                            ClassName = s.ClassName,
                            CreateTime = DateTime.UtcNow,
                            TeacherId = teacher.Id
                        }, ct);
                    }
                }
            }, ct);

            return teacher.Id;
        }

        /// <summary>
        /// 演示嵌套调用（内层方法检测到已有事务，不再重复开启）
        /// </summary>
        public async Task UpdateTeacherAndAddStudentsAsync(
            int teacherId,
            IEnumerable<CreateStudentItem> newStudents,
            CancellationToken ct = default)
        {
            await _uow.ExecuteInTransactionAsync(async repo =>
            {
                var teacher = await repo.GetFirstOrDefaultAsync<TeacherInfo>(x => x.Id == teacherId, ct);
                if (teacher == null) throw new InvalidOperationException("Teacher not found.");

                teacher.UpdateTime = DateTime.UtcNow;
                teacher.CourseName = teacher.CourseName + " (Updated)";
                await repo.UpdateAsync(teacher, ct);

                // 调用内层方法（不会重复开事务）
                await AddStudentsInternalAsync(teacherId, newStudents, ct);

            }, ct);
        }

        private async Task AddStudentsInternalAsync(int teacherId, IEnumerable<CreateStudentItem> students, CancellationToken ct)
        {
            if (students == null) return;

            foreach (var s in students)
            {
                await _uow.Repository.AddAsync(new StudentInfo
                {
                    StudentName = s.StudentName,
                    Age = s.Age,
                    ClassName = s.ClassName,
                    CreateTime = DateTime.UtcNow,
                    TeacherId = teacherId
                }, ct);
            }
        }

        private static void Validate(CreateTeacherRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.TeacherName))
                throw new ValidationException("TeacherName 不能为空");
        }
    }
}