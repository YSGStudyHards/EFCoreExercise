//using EFCoreGenericRepository.Interfaces;
//using Entity;
//using Entity.DBModel;
//using Entity.ViewModel;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;

//namespace WebAPI.Controllers
//{
//    /// <summary>
//    /// StudentInfoRepositoryController
//    /// </summary>
//    [Route("api/[controller]")]
//    public class StudentInfoRepositoryController
//    {
//        private readonly IRepository _repository;

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="repository">repository</param>
//        public StudentInfoRepositoryController(IRepository repository)
//        {
//            _repository = repository;
//        }

//        #region 事务管理操作

//        /// <summary>
//        /// 在事务中执行批量操作示例
//        /// </summary>
//        /// <param name="students">学生信息列表</param>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<ApiResponse<List<StudentInfo>>> BatchAddInTransactionAsync([FromBody] List<StudentInfo> students)
//        {
//            try
//            {
//                var result = await _repository.ExecuteInTransactionAsync(async () =>
//                {
//                    await _repository.AddRangeAsync(students);
//                    await _repository.SaveChangesAsync();
//                    return students;
//                });

//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = true,
//                    Message = "批量添加学生成功",
//                    Data = result
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"批量添加学生失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        #endregion

//        #region 数据查询操作

//        /// <summary>
//        /// 获取学生查询对象（用于复杂查询）
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("queryable")]
//        public async Task<ApiResponse<List<StudentInfo>>> GetQueryable()
//        {
//            try
//            {
//                var queryable = _repository.GetQueryable<StudentInfo>();

//                // 构建复杂查询
//                var result = await queryable
//                    .Where(s => s.Gender == 1)
//                    .OrderBy(s => s.StudentName)
//                    .Skip(10)
//                    .Take(20)
//                    .ToListAsync();

//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = true,
//                    Data = result
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"获取查询对象失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 根据学生ID获取学生信息
//        /// </summary>
//        /// <param name="studentId">学生ID</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("{studentId:int}")]
//        public async Task<ApiResponse<StudentInfo>> GetByIdAsync(int studentId)
//        {
//            try
//            {
//                var student = await _repository.GetByIdAsync<StudentInfo, int>(studentId);

//                if (student == null)
//                {
//                    return new ApiResponse<StudentInfo>
//                    {
//                        Success = false,
//                        Message = "学生不存在",
//                        Data = null
//                    };
//                }

//                return new ApiResponse<StudentInfo>
//                {
//                    Success = true,
//                    Message = "获取学生信息成功",
//                    Data = student
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<StudentInfo>
//                {
//                    Success = false,
//                    Message = $"获取学生信息失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 根据学生姓名获取第一个匹配的学生
//        /// </summary>
//        /// <param name="studentName">学生姓名</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("by-name/{studentName}")]
//        public async Task<ApiResponse<StudentInfo>> GetFirstByNameAsync(
//            string studentName,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var student = await _repository.GetFirstOrDefaultAsync<StudentInfo>(
//                    s => s.StudentName == studentName,
//                    cancellationToken);

//                return new ApiResponse<StudentInfo>
//                {
//                    Success = true,
//                    Message = student != null ? "获取学生信息成功" : "未找到匹配的学生",
//                    Data = student
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<StudentInfo>
//                {
//                    Success = false,
//                    Message = $"获取学生信息失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 根据班级ID获取学生（包含班级信息）
//        /// </summary>
//        /// <param name="classId">班级ID</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("by-class/{classId:int}/with-class-info")]
//        public async Task<ApiResponse<StudentInfo>> GetFirstByClassWithClassInfoAsync(
//            int classId,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var student = await _repository.GetFirstOrDefaultAsync<StudentInfo>(
//                    s => s.ClassID == classId,
//                    query => query.Include(s => s.ClassInfo),
//                    cancellationToken);

//                return new ApiResponse<StudentInfo>
//                {
//                    Success = true,
//                    Message = student != null ? "获取学生信息成功" : "未找到匹配的学生",
//                    Data = student
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<StudentInfo>
//                {
//                    Success = false,
//                    Message = $"获取学生信息失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 获取所有学生信息
//        /// </summary>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("all")]
//        public async Task<ApiResponse<List<StudentInfo>>> GetAllAsync(CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var students = await _repository.GetAllAsync<StudentInfo>(cancellationToken);

//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = true,
//                    Message = "获取所有学生信息成功",
//                    Data = students
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"获取学生信息失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 根据性别获取学生列表
//        /// </summary>
//        /// <param name="gender">性别（1男生，2女生）</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("by-gender/{gender:int}")]
//        public async Task<ApiResponse<List<StudentInfo>>> GetListByGenderAsync(
//            int gender,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var students = await _repository.GetListAsync<StudentInfo>(
//                    s => s.Gender == gender,
//                    cancellationToken);

//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = true,
//                    Message = "获取学生列表成功",
//                    Data = students
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"获取学生列表失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 根据班级ID获取学生列表（包含班级信息）
//        /// </summary>
//        /// <param name="classId">班级ID</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("by-class/{classId:int}")]
//        public async Task<ApiResponse<List<StudentInfo>>> GetListByClassWithClassInfoAsync(
//            int classId,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var students = await _repository.GetListAsync<StudentInfo>(
//                    s => s.ClassID == classId,
//                    query => query.Include(s => s.ClassInfo),
//                    cancellationToken);

//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = true,
//                    Message = "获取班级学生列表成功",
//                    Data = students
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"获取班级学生列表失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 分页获取学生信息
//        /// </summary>
//        /// <param name="pageIndex">页索引（从0开始）</param>
//        /// <param name="pageSize">页大小</param>
//        /// <param name="gender">性别筛选（可选）</param>
//        /// <param name="classId">班级ID筛选（可选）</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("paged")]
//        public async Task<ApiResponse<PagedResult<StudentInfo>>> GetPagedAsync(
//            [FromQuery] int pageIndex = 0,
//            [FromQuery] int pageSize = 10,
//            [FromQuery] int? gender = null,
//            [FromQuery] int? classId = null,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                Expression<Func<StudentInfo, bool>> predicate = null;

//                // 构建筛选条件
//                if (gender.HasValue && classId.HasValue)
//                {
//                    predicate = s => s.Gender == gender.Value && s.ClassID == classId.Value;
//                }
//                else if (gender.HasValue)
//                {
//                    predicate = s => s.Gender == gender.Value;
//                }
//                else if (classId.HasValue)
//                {
//                    predicate = s => s.ClassID == classId.Value;
//                }

//                var pagedResult = await _repository.GetPagedAsync<StudentInfo>(
//                    pageIndex,
//                    pageSize,
//                    predicate,
//                    query => query.OrderBy(s => s.StudentID),
//                    cancellationToken);

//                return new ApiResponse<PagedResult<StudentInfo>>
//                {
//                    Success = true,
//                    Message = "获取分页学生信息成功",
//                    Data = pagedResult
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<PagedResult<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"获取分页学生信息失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 检查学生是否存在
//        /// </summary>
//        /// <param name="studentName">学生姓名</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("exists/{studentName}")]
//        public async Task<ApiResponse<bool>> ExistsAsync(
//            string studentName,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var exists = await _repository.ExistsAsync<StudentInfo>(
//                    s => s.StudentName == studentName,
//                    cancellationToken);

//                return new ApiResponse<bool>
//                {
//                    Success = true,
//                    Message = "检查学生存在性成功",
//                    Data = exists
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<bool>
//                {
//                    Success = false,
//                    Message = $"检查学生存在性失败：{ex.Message}",
//                    Data = false
//                };
//            }
//        }

//        /// <summary>
//        /// 获取学生数量
//        /// </summary>
//        /// <param name="gender">性别筛选（可选）</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpGet("count")]
//        public async Task<ApiResponse<int>> CountAsync(
//            [FromQuery] int? gender = null,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                Expression<Func<StudentInfo, bool>> predicate = null;
//                if (gender.HasValue)
//                {
//                    predicate = s => s.Gender == gender.Value;
//                }

//                var count = await _repository.CountAsync<StudentInfo>(predicate, cancellationToken);

//                return new ApiResponse<int>
//                {
//                    Success = true,
//                    Message = "获取学生数量成功",
//                    Data = count
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<int>
//                {
//                    Success = false,
//                    Message = $"获取学生数量失败：{ex.Message}",
//                    Data = 0
//                };
//            }
//        }

//        #endregion

//        #region 增删改操作

//        /// <summary>
//        /// 添加学生
//        /// </summary>
//        /// <param name="student">学生信息</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<ApiResponse<StudentInfo>> AddAsync(
//            [FromBody] StudentInfo student,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                // 设置创建时间
//                student.CreateTime = DateTime.UtcNow;

//                var addedStudent = await _repository.AddAsync(student, cancellationToken);
//                await _repository.SaveChangesAsync(cancellationToken);

//                return new ApiResponse<StudentInfo>
//                {
//                    Success = true,
//                    Message = "添加学生成功",
//                    Data = addedStudent
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<StudentInfo>
//                {
//                    Success = false,
//                    Message = $"添加学生失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 批量添加学生
//        /// </summary>
//        /// <param name="students">学生信息列表</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpPost("batch")]
//        public async Task<ApiResponse<List<StudentInfo>>> AddRangeAsync(
//            [FromBody] List<StudentInfo> students,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                // 设置创建时间
//                var currentTime = DateTime.UtcNow;
//                students.ForEach(s => s.CreateTime = currentTime);

//                await _repository.AddRangeAsync(students, cancellationToken);
//                await _repository.SaveChangesAsync(cancellationToken);

//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = true,
//                    Message = "批量添加学生成功",
//                    Data = students
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"批量添加学生失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 更新学生信息
//        /// </summary>
//        /// <param name="student">学生信息</param>
//        /// <returns></returns>
//        [HttpPut]
//        public async Task<ApiResponse<StudentInfo>> UpdateAsync([FromBody] StudentInfo student)
//        {
//            try
//            {
//                // 设置更新时间
//                student.UpdateTime = DateTime.UtcNow;

//                var updatedStudent = _repository.Update(student);
//                await _repository.SaveChangesAsync();

//                return new ApiResponse<StudentInfo>
//                {
//                    Success = true,
//                    Message = "更新学生信息成功",
//                    Data = updatedStudent
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<StudentInfo>
//                {
//                    Success = false,
//                    Message = $"更新学生信息失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 批量更新学生信息
//        /// </summary>
//        /// <param name="students">学生信息列表</param>
//        /// <returns></returns>
//        [HttpPut("batch")]
//        public async Task<ApiResponse<List<StudentInfo>>> UpdateRangeAsync([FromBody] List<StudentInfo> students)
//        {
//            try
//            {
//                // 设置更新时间
//                var currentTime = DateTime.UtcNow;
//                students.ForEach(s => s.UpdateTime = currentTime);

//                _repository.UpdateRange(students);
//                await _repository.SaveChangesAsync();

//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = true,
//                    Message = "批量更新学生信息成功",
//                    Data = students
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<List<StudentInfo>>
//                {
//                    Success = false,
//                    Message = $"批量更新学生信息失败：{ex.Message}",
//                    Data = null
//                };
//            }
//        }

//        /// <summary>
//        /// 删除学生（通过实体）
//        /// </summary>
//        /// <param name="student">学生信息</param>
//        /// <returns></returns>
//        [HttpDelete("by-entity")]
//        public async Task<ApiResponse<bool>> DeleteAsync([FromBody] StudentInfo student)
//        {
//            try
//            {
//                _repository.Delete(student);
//                await _repository.SaveChangesAsync();

//                return new ApiResponse<bool>
//                {
//                    Success = true,
//                    Message = "删除学生成功",
//                    Data = true
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<bool>
//                {
//                    Success = false,
//                    Message = $"删除学生失败：{ex.Message}",
//                    Data = false
//                };
//            }
//        }

//        /// <summary>
//        /// 根据ID删除学生
//        /// </summary>
//        /// <param name="studentId">学生ID</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpDelete("{studentId:int}")]
//        public async Task<ApiResponse<bool>> DeleteByIdAsync(
//            int studentId,
//            CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                await _repository.DeleteByIdAsync<StudentInfo, int>(studentId, cancellationToken);
//                await _repository.SaveChangesAsync(cancellationToken);

//                return new ApiResponse<bool>
//                {
//                    Success = true,
//                    Message = "删除学生成功",
//                    Data = true
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<bool>
//                {
//                    Success = false,
//                    Message = $"删除学生失败：{ex.Message}",
//                    Data = false
//                };
//            }
//        }

//        /// <summary>
//        /// 批量删除学生
//        /// </summary>
//        /// <param name="students">学生信息列表</param>
//        /// <returns></returns>
//        [HttpDelete("batch")]
//        public async Task<ApiResponse<bool>> DeleteRangeAsync([FromBody] List<StudentInfo> students)
//        {
//            try
//            {
//                _repository.DeleteRange(students);
//                await _repository.SaveChangesAsync();

//                return new ApiResponse<bool>
//                {
//                    Success = true,
//                    Message = "批量删除学生成功",
//                    Data = true
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<bool>
//                {
//                    Success = false,
//                    Message = $"批量删除学生失败：{ex.Message}",
//                    Data = false
//                };
//            }
//        }

//        /// <summary>
//        /// 保存更改（手动触发）
//        /// </summary>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns></returns>
//        [HttpPost("save-changes")]
//        public async Task<ApiResponse<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var changes = await _repository.SaveChangesAsync(cancellationToken);

//                return new ApiResponse<int>
//                {
//                    Success = true,
//                    Message = "保存更改成功",
//                    Data = changes
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse<int>
//                {
//                    Success = false,
//                    Message = $"保存更改失败：{ex.Message}",
//                    Data = 0
//                };
//            }
//        }

//        #endregion

//    }
//}
