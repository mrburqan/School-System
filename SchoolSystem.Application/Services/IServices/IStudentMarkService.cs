using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface IStudentMarkService
    {
        Task<ResponseData<StudentMarkModelDto>> GetByAsync(StudentMarkModelDto model);
        Task<ResponseData<bool>> DeleteRangeAsync();

        Task<ResponseData<StudentMarkModelDto>> AddAsync(StudentMarkModel model);
        Task<ResponseData<bool>> DeleteFirstOrDefaultAsync(int id);
        Task<ResponseData<StudentMarkModelDto>> UpdateAsync(StudentMarkModel model);
        Task<ResponseData<PagedResponse<StudentMarkModelDto>>> GetPagedAsync(PageRequest request);
    }
}
