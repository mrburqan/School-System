using SchoolSystem.Application.Models;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface IAssociationService
    {
        Task<ResponseData<bool>> AddAssociationAsync(AssociationModel association);
        Task<ResponseData<IEnumerable<AssociationModelDto>>> GetAssociationAsync(int? userID, int? classId);
        Task<ResponseData<bool>> CheckIfEnrolledAsync(int userId, int classId);
        Task<ResponseData<AssociationModelDto>> UpdateAssociationAsync(AssociationModel association);
        Task<ResponseData<bool>> DeleteAssociationAsync(int userId, int classId);
        Task<ResponseData<PagedResponse<AssociationModelDto>>> GetPagedAsync(PageRequest pageRequest);

    }
}
