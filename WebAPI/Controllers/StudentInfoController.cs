using Entity;
using Entity.DBModel;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 学生信息管理
    /// </summary>
    [Route("api/[controller]")]
    public class StudentInfoController
    {
        private readonly SchoolDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">context</param>
        public StudentInfoController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 添加学生信息
        /// </summary>
        /// <param name="model">学生信息Model</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<int>> Create([FromBody] StudentViewModel model)
        {
            try
            {
                var classExist = await _context.Classes.FirstOrDefaultAsync(c => c.ClassID == model.ClassID);
                if (classExist == null)
                {
                    return new ApiResponse<int>
                    {
                        Success = false,
                        Message = "无效的班级ID"
                    };
                }

                var student = new StudentInfo
                {
                    StudentName = model.StudentName,
                    Gender = model.Gender,
                    Birthday = model.Birthday,
                    ClassID = model.ClassID,
                    ParentPhone = model.ParentPhone,
                    Address = model.Address,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                _context.Students.Add(student);
                var addResult = await _context.SaveChangesAsync();

                if (addResult > 0)
                {
                    return new ApiResponse<int>
                    {
                        Success = true,
                        Data = student.StudentID,
                        Message = "创建成功"
                    };
                }
                else
                {
                    return new ApiResponse<int>
                    {
                        Success = false,
                        Message = "创建失败"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取所有学生信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<StudentViewModel>>> GetAllStudentInfo()
        {
            try
            {
                var students = await _context.Students.Select(s => new StudentViewModel
                {
                    StudentID = s.StudentID,
                    StudentName = s.StudentName,
                    Gender = s.Gender,
                    Birthday = s.Birthday,
                    ClassID = s.ClassID,
                    ParentPhone = s.ParentPhone,
                    Address = s.Address
                }).ToListAsync();

                return new ApiResponse<List<StudentViewModel>>
                {
                    Success = true,
                    Data = students
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<StudentViewModel>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取指定学生信息
        /// </summary>
        /// <param name="id">学生编号</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<StudentViewModel>> GetStudentInfoById(int id)
        {
            try
            {
                var studentInfo = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == id);
                if (studentInfo == null)
                {
                    return new ApiResponse<StudentViewModel>
                    {
                        Success = false,
                        Message = "学生不存在"
                    };
                }

                var result = new StudentViewModel
                {
                    StudentID = studentInfo.StudentID,
                    StudentName = studentInfo.StudentName,
                    Gender = studentInfo.Gender,
                    Birthday = studentInfo.Birthday,
                    ClassID = studentInfo.ClassID,
                    ParentPhone = studentInfo.ParentPhone,
                    Address = studentInfo.Address
                };

                return new ApiResponse<StudentViewModel>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<StudentViewModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 更新学生信息
        /// </summary>
        /// <param name="id">学生编号</param>
        /// <param name="model">学生信息Model</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateStudentInfo(int id, [FromBody] StudentViewModel model)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(x => x.StudentID == id);
                if (student == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"编号为 {id} 的学生不存在"
                    };
                }

                // 更新字段
                student.StudentName = model.StudentName;
                student.Gender = model.Gender;
                student.Birthday = model.Birthday;
                student.ClassID = model.ClassID;
                student.ParentPhone = model.ParentPhone;
                student.Address = model.Address;
                student.UpdateTime = DateTime.Now;
                var updateResult = await _context.SaveChangesAsync();

                if (updateResult > 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "更新成功"
                    };
                }
                else
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "更新失败"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 删除学生信息
        /// </summary>
        /// <param name="id">学生编号</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteStudentInfo(int id)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(x => x.StudentID == id);
                if (student == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"编号为 {id} 的学生不存在"
                    };
                }

                _context.Students.Remove(student);
                var deleteResult = await _context.SaveChangesAsync();

                if (deleteResult > 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "删除成功"
                    };
                }
                else
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "删除失败"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
