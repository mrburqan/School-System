using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface IGenderService
    {
        Task<ResponseData<GenderModel>> AddAsync(GenderModel model);
        Task<ResponseData<GenderModelDto>> UpdateAsync(GenderModel model);
        Task<ResponseData<GenderModelDto>> GetFirstOrDefaultAsync(GenderModelDto model);
        Task<ResponseData<PagedResponse<GenderModelDto>>> GetPageAsync(PageRequest request);

    }
}
