using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface IClientService
    {
        Task<ResponseData<ClientModelDto>> GetByFirstOrDefult(ClientModelDto model);
        Task<ResponseData<ClientModelDto>> AddAsync(ClientModel model);
        Task<ResponseData<bool>> DeleteFirstOrDefaultAsync(int id);
        Task<ResponseData<ClientModelDto>> UpdateAsync(ClientModel model);
        Task<ResponseData<PagedResponse<ClientModelDto>>> GetPagedAsync(PageRequest request);


    }
}