using EFCoreGenericRepository.Interfaces;
using Entity;
using Entity.DBModel;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 老师信息管理
    /// </summary>
    [Route("api/[controller]")]
    public class TeacherInfoRepositoryController
    {
        private readonly IRepository _repository;

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="repository">repository</param>
        public TeacherInfoRepositoryController(IRepository repository)
        {
            _repository = repository;
        }

        #region 查询操作

        /// <summary>
        /// 根据ID获取教师信息
        /// </summary>
        /// <param name="id">教师ID</param>
        /// <returns>教师信息</returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<TeacherInfo>> GetById([FromRoute] int id)
        {
            var teacher = await _repository.GetByIdAsync<TeacherInfo>(id).ConfigureAwait(false);
            if (teacher == null)
            {
                return new ApiResponse<TeacherInfo>
                {
                    Success = true,
                    Data = teacher ?? new TeacherInfo()
                };
            }
            else
            {
                return new ApiResponse<TeacherInfo>
                {
                    Success = false,
                    Message = $"未找到ID为{id}的教师信息"
                };
            }
        }

        /// <summary>
        /// 获取所有教师信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<TeacherInfo>>> GetAllTeacherInfos()
        {
            var teachers = await _repository.GetAllAsync<TeacherInfo>().ConfigureAwait(false);
            return new ApiResponse<List<TeacherInfo>>
            {
                Success = true,
                Data = teachers
            };
        }

        /// <summary>
        /// 根据教师姓名或者邮箱获取教师信息
        /// </summary>
        /// <param name="name">教师姓名</param>
        /// <param name="email">教师邮箱</param>
        /// <returns>匹配的教师信息</returns>
        [HttpGet("GetByTeacherNameOrEmail")]
        public async Task<ApiResponse<TeacherInfo>> GetByTeacherNameOrEmail([FromQuery] string name, [FromQuery] string email)
        {
            var teacher = await _repository.GetFirstOrDefaultAsync<TeacherInfo>(x => x.TeacherName == name || x.Email == email).ConfigureAwait(false);

            if (teacher == null)
            {
                return new ApiResponse<TeacherInfo>
                {
                    Success = true,
                    Data = teacher ?? new TeacherInfo()
                };
            }
            else
            {
                return new ApiResponse<TeacherInfo>
                {
                    Success = false,
                    Message = $"未找到的教师信息"
                };
            }
        }

        /// <summary>
        /// 分页查询教师信息
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="pageIndex">页索引（从0开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页教师信息</returns>
        [HttpGet("GetPagedList")]
        public async Task<ApiResponse<EFCoreGenericRepository.Models.PagedResult<TeacherInfo>>> GetPagedList(
            [FromQuery] string name,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _repository.GetPagedAsync<TeacherInfo>(
                pageIndex,
                pageSize,
                !string.IsNullOrWhiteSpace(name) ? t => t.TeacherName == name : null,
                query => query.OrderBy(t => t.TeacherID)).ConfigureAwait(false);

            return new ApiResponse<EFCoreGenericRepository.Models.PagedResult<TeacherInfo>>
            {
                Success = true,
                Data = result
            };
        }

        /// <summary>
        /// 检查教师是否存在
        /// </summary>
        /// <param name="id">教师ID</param>
        /// <returns>是否存在</returns>
        [HttpGet("Exists")]
        public async Task<ApiResponse<string>> Exists([FromQuery] int id)
        {
            var exists = await _repository.ExistsAsync<TeacherInfo>(t => t.TeacherID == id).ConfigureAwait(false);

            return new ApiResponse<string>
            {
                Success = true,
                Message = exists ? "教师存在" : "教师不存在"
            };
        }

        /// <summary>
        /// 获取教师总数
        /// </summary>
        /// <param name="gender">性别筛选（可选）</param>
        /// <returns>教师总数</returns>
        [HttpGet("GetCount")]
        public async Task<ApiResponse<int>> GetCount([FromQuery] int? gender = null)
        {
            var count = await _repository.CountAsync<TeacherInfo>(gender.HasValue ? t => t.Gender == gender.Value : null).ConfigureAwait(false);

            return new ApiResponse<int>
            {
                Success = true,
                Message = $"总数：{count}"
            };
        }

        #endregion

        #region 增删改操作

        /// <summary>
        /// 创建新教师信息
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        [HttpPost("CreateTeacherInfo")]
        public async Task<ApiResponse<bool>> CreateTeacherInfo([FromBody] CreateTeacherRequest request)
        {
            var teacher = new TeacherInfo
            {
                TeacherName = request.TeacherName,
                Gender = request.Gender,
                Birthday = request.Birthday,
                Phone = request.Phone,
                Email = request.Email,
                UpdateTime = DateTime.Now,
                CreateTime = DateTime.Now
            };

            var createResult = await _repository.AddAsync(teacher).ConfigureAwait(false);
            if (createResult > 0)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "添加成功"
                };
            }
            else
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "添加失败"
                };
            }
        }

        /// <summary>
        /// 更新教师信息
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        [HttpPost("UpdateTeacherInfo")]
        public async Task<ApiResponse<bool>> UpdateTeacherInfo(TeacherInfo request)
        {
            var teacher = await _repository.GetByIdAsync<TeacherInfo>(request.TeacherID);
            if (teacher == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"未找到ID为{request.TeacherID}的教师"
                };
            }

            var updateResult = await _repository.UpdateAsync(teacher).ConfigureAwait(false);
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

        /// <summary>
        /// 删除教师信息
        /// </summary>
        /// <param name="id">教师id</param>
        /// <returns></returns>
        [HttpPost("DeleteTeacherInfo")]
        public async Task<ApiResponse<bool>> DeleteTeacherInfo([FromQuery] int id)
        {
            var deleteResult = await _repository.DeleteByIdAsync<TeacherInfo>(id);
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

        #endregion
    }
}
