using EFCoreGenericRepository.Interfaces;
using Entity.DBModel;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Service
{
    /// <summary>
    /// ��ʾ���� UnitOfWork �������Բ�������
    /// </summary>
    public class TeacherUnitOfWorkService
    {
        private readonly IUnitOfWork<SchoolDbContext> _uow;

        public TeacherUnitOfWorkService(IUnitOfWork<SchoolDbContext> uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// ��ʽ�ֶ� Begin / Commit / Rollback ��ʽ
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

                // �ӳٱ��� -> Commit �ڲ� SaveChanges + Commit
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
        /// ʹ�� ExecuteInTransactionAsync ��װ������ģ����룩
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
        /// ��ʾǶ�׵��ã��ڲ㷽����⵽�������񣬲����ظ�������
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

                // �����ڲ㷽���������ظ�������
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
                throw new ValidationException("TeacherName ����Ϊ��");
        }
    }
}