using EFCoreGenericRepository.Interfaces;
using Entity;
using Entity.DBModel;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 工作单元操作演示
    /// 演示两种事务使用模式：
    /// 1. 包装事务：ExecuteInTransactionAsync (推荐，减少样板代码)
    /// 2. 显式事务：BeginTransactionAsync + CommitAsync + RollbackAsync
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class TeacherUnitOfWorkController : ControllerBase
    {
        private readonly IUnitOfWork<SchoolDbContext> _uow;

        /// <summary>
        /// 依赖注入工作单元
        /// </summary>
        /// <param name="uow">uow</param>
        public TeacherUnitOfWorkController(IUnitOfWork<SchoolDbContext> uow)
        {
            _uow = uow;
        }

        #region 包装事务示例（ExecuteInTransactionAsync）推荐方式

        /// <summary>
        /// 创建老师信息
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateTeacher([FromBody] CreateTeacherRequest request)
        {
            try
            {
                await _uow.ExecuteInTransactionAsync(async repo =>
                {
                    var teacher = new TeacherInfo
                    {
                        TeacherName = request.TeacherName,
                        Gender = request.Gender,
                        Birthday = request.Birthday,
                        Phone = request.Phone,
                        Email = request.Email,
                        CreateTime = DateTime.UtcNow
                    };
                    await repo.AddAsync(teacher).ConfigureAwait(false);
                }).ConfigureAwait(false);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "创建老师信息成功（单事务）"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object> { Success = false, Message = "操作失败：" + ex.Message });
            }
        }

        /// <summary>
        /// 使用 ExecuteInTransactionAsync 创建教师及其学生
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateTeacherWithStudents([FromBody] CreateTeacherWithStudentsRequest request)
        {
            try
            {
                await _uow.ExecuteInTransactionAsync(async repo =>
                {
                    var teacher = new TeacherInfo
                    {
                        TeacherName = request.TeacherName,
                        Gender = request.Gender,
                        Birthday = request.Birthday,
                        Phone = request.Phone,
                        Email = request.Email,
                        CreateTime = DateTime.UtcNow
                    };
                    await repo.AddAsync(teacher).ConfigureAwait(false);

                    foreach (var s in request.Students)
                    {
                        await repo.AddAsync(new StudentInfo
                        {
                            StudentName = s.StudentName,
                            Gender = s.Gender,
                            Birthday = s.Birthday,
                            ClassID = s.ClassID,
                            ParentPhone = s.ParentPhone,
                            Address = s.Address,
                            CreateTime = DateTime.UtcNow
                        }).ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "创建教师及学生成功（包装事务）"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object> { Success = false, Message = "操作失败：" + ex.Message });
            }
        }

        #endregion

        #region 显式事务示例（手动 Begin / Commit / Rollback）

        /// <summary>
        /// 显式事务：更新教师联系方式并批量新增学生
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> UpdateTeacherAndAddStudents([FromBody] CreateTeacherWithStudentsRequest request)
        {
            await _uow.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var teacher = await _uow.Repository.GetFirstOrDefaultAsync<TeacherInfo>(t => t.TeacherName == request.TeacherName).ConfigureAwait(false);
                if (teacher == null)
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Teacher {request.TeacherName} 不存在"
                    });

                // 更新联系方式
                teacher.Phone = request.Phone;
                teacher.Email = request.Email;
                teacher.UpdateTime = DateTime.UtcNow;
                await _uow.Repository.UpdateAsync(teacher).ConfigureAwait(false);

                // 批量新增学生（如果有）
                foreach (var s in request.Students)
                {
                    await _uow.Repository.AddAsync(new StudentInfo
                    {
                        StudentName = s.StudentName,
                        Gender = s.Gender,
                        Birthday = s.Birthday,
                        ClassID = s.ClassID,
                        ParentPhone = s.ParentPhone,
                        Address = s.Address,
                        CreateTime = DateTime.UtcNow
                    }).ConfigureAwait(false);
                }

                await _uow.CommitAsync().ConfigureAwait(false);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "更新教师并新增学生成功（显式事务）"
                });
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync().ConfigureAwait(false);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object> { Success = false, Message = "操作失败：" + ex.Message });
            }
        }

        #endregion
    }
}