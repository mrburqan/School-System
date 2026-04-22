using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface ITypeService
    {
        Task<ResponseData<TypeModel>> AddAsync(TypeModel model);
        Task<ResponseData<TypeModelDto>> GetFirstOrDefaultAsync(TypeModelDto model);
        Task<ResponseData<TypeModelDto>> UpdateAsync(TypeModel model);
        Task<ResponseData<PagedResponse<TypeModelDto>>> GetPageAsync(PageRequest request);
    }
}
