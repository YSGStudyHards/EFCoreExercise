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
        private readonly IUnitOfWork<SchoolDbContext> _uow; // 用于演示直接使用方式

        public TeacherUnitOfWorkController(
            TeacherUnitOfWorkService service,
            IUnitOfWork<SchoolDbContext> uow)
        {
            _service = service;
            _uow = uow;
        }

        /// <summary>
        /// 显式事务调用示例
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExplicit(CreateTeacherRequest request, CancellationToken ct)
        {
            var id = await _service.CreateTeacherWithStudentsExplicitAsync(request, ct);
            return Ok(new { TeacherId = id, Mode = "Explicit" });
        }

        /// <summary>
        /// 包装器方式（ExecuteInTransactionAsync）
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateWrapped(CreateTeacherRequest request, CancellationToken ct)
        {
            var id = await _service.CreateTeacherWithStudentsWrappedAsync(request, ct);
            return Ok(new { TeacherId = id, Mode = "Wrapped" });
        }

        /// <summary>
        /// 演示直接使用 UoW（不通过服务层）——不推荐在复杂业务中直接放控制器
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