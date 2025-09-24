using EFCoreGenericRepository.Interfaces;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TeacherUnitOfWorkController : ControllerBase
    {
        private readonly TeacherUnitOfWorkService _service;
        private readonly IUnitOfWork<SchoolDbContext> _uow; // ������ʾֱ��ʹ�÷�ʽ

        public TeacherUnitOfWorkController(
            TeacherUnitOfWorkService service,
            IUnitOfWork<SchoolDbContext> uow)
        {
            _service = service;
            _uow = uow;
        }

        /// <summary>
        /// ��ʽ�������ʾ��
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExplicit(CreateTeacherRequest request, CancellationToken ct)
        {
            var id = await _service.CreateTeacherWithStudentsExplicitAsync(request, ct);
            return Ok(new { TeacherId = id, Mode = "Explicit" });
        }

        /// <summary>
        /// ��װ����ʽ��ExecuteInTransactionAsync��
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateWrapped(CreateTeacherRequest request, CancellationToken ct)
        {
            var id = await _service.CreateTeacherWithStudentsWrappedAsync(request, ct);
            return Ok(new { TeacherId = id, Mode = "Wrapped" });
        }

        /// <summary>
        /// ��ʾֱ��ʹ�� UoW����ͨ������㣩�������Ƽ��ڸ���ҵ����ֱ�ӷſ�����
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDirect(CreateTeacherRequest request, CancellationToken ct)
        {
            await _uow.BeginTransactionAsync(ct);
            try
            {
                var teacher = new Entity.DBModel.TeacherInfo
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
                        await _uow.Repository.AddAsync(new Entity.DBModel.StudentInfo
                        {
                            StudentName = s.StudentName,
                            Age = s.Age,
                            ClassName = s.ClassName,
                            CreateTime = DateTime.UtcNow,
                            TeacherId = teacher.Id
                        }, ct);
                    }
                }

                await _uow.CommitAsync(ct);
                return Ok(new { TeacherId = teacher.Id, Mode = "Direct" });
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }
    }
}