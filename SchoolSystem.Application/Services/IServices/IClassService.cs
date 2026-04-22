using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface IClassService
    {
        Task<ResponseData<ClassModelDto>> AddAsync(ClassModel model);
        Task<ResponseData<ClassModelDto>> UpdateAsync(ClassModel model);
        Task<ResponseData<bool>> DeleteFirstOrDefaultAsync(int id);
        Task<ResponseData<ClassModelDto>> GetFirstOrDefaulteAsync(ClassModelDto data);
        Task<ResponseData<PagedResponse<ClassModelDto>>> GetPagedAsync(PageRequest pageRequest);
    }
}
