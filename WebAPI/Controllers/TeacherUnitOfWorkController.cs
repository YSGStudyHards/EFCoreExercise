using EFCoreGenericRepository.Interfaces;
using Entity;
using Entity.DBModel;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebAPI.Controllers
{
    /// <summary>
    /// ������Ԫ������ʾ
    /// ��ʾ��������ʹ��ģʽ��
    /// 1. ��װ����ExecuteInTransactionAsync (�Ƽ��������������)
    /// 2. ��ʽ����BeginTransactionAsync + CommitAsync + RollbackAsync
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class TeacherUnitOfWorkController : ControllerBase
    {
        private readonly IUnitOfWork<SchoolDbContext> _uow;

        /// <summary>
        /// ����ע�빤����Ԫ
        /// </summary>
        /// <param name="uow">uow</param>
        public TeacherUnitOfWorkController(IUnitOfWork<SchoolDbContext> uow)
        {
            _uow = uow;
        }

        #region ��װ����ʾ����ExecuteInTransactionAsync���Ƽ���ʽ

        /// <summary>
        /// ������ʦ��Ϣ
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
                    Message = "������ʦ��Ϣ�ɹ���������"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object> { Success = false, Message = "����ʧ�ܣ�" + ex.Message });
            }
        }

        /// <summary>
        /// ʹ�� ExecuteInTransactionAsync ������ʦ����ѧ��
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
                    Message = "������ʦ��ѧ���ɹ�����װ����"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object> { Success = false, Message = "����ʧ�ܣ�" + ex.Message });
            }
        }

        #endregion

        #region ��ʽ����ʾ�����ֶ� Begin / Commit / Rollback��

        /// <summary>
        /// ��ʽ���񣺸��½�ʦ��ϵ��ʽ����������ѧ��
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
                        Message = $"Teacher {request.TeacherName} ������"
                    });

                // ������ϵ��ʽ
                teacher.Phone = request.Phone;
                teacher.Email = request.Email;
                teacher.UpdateTime = DateTime.UtcNow;
                await _uow.Repository.UpdateAsync(teacher).ConfigureAwait(false);

                // ��������ѧ��������У�
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
                    Message = "���½�ʦ������ѧ���ɹ�����ʽ����"
                });
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync().ConfigureAwait(false);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object> { Success = false, Message = "����ʧ�ܣ�" + ex.Message });
            }
        }

        #endregion
    }
}