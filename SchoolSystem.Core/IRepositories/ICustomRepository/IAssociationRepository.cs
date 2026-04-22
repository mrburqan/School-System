using SchoolSystem.Core.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolSystem.Core.IRepositories.ICustomRepository
{
    public interface IAssociationRepository
    {
        Task<IEnumerable<Ismail_Clients>> GetAssociationsAsync(int? classId, int? userId);
        Task<bool> AddAssociationAsync(int userId, int classId);
        Task<bool> DeleteAssociationAsync(int userId, int classId);
        Task<bool> ExistsAsync(int userId, int classId);
        Task<bool> UpdateAssociationAsync(int userId, int oldClassId, int newClassId);
    }
}
