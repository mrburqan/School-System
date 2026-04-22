using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface IErrorLogsService
    {
        Task<ResponseData<bool>> AddErrorLogAsync(ErrorLogsModel errorLogs);
        Task<ResponseData<PagedResponse<ErrorLogsModelDto>>> GetPagedAsync(PageRequest request);

        Task<ResponseData<bool>> ResolveAndDeleteAsync(int errorId);

        Task<ResponseData<bool>> ClearAllLogsAsync();
    }
}
