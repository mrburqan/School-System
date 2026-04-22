using SchoolSystem.Core.Entites;
using SchoolSystem.Core.IRepositories.ICustomRepository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Repositories.CustomRepository
{
    public class AssociationRepository : GenericRepository<Ismail_Clients>, IAssociationRepository
    {

        public AssociationRepository(SchoolSystemEntities dbContext) : base(dbContext)
        {
        }

        public async Task<bool> AddAssociationAsync(int classId, int clientId)
        {
            var client = await _context.Ismail_Clients
                .Include(c => c.Ismail_Classes)
                .FirstOrDefaultAsync(c => c.UserID == clientId);

            var schoolClass = await _context.Ismail_Classes.FirstOrDefaultAsync(cls => cls.ClassID == classId);

            if (schoolClass != null && client != null)
            {
                if (!client.Ismail_Classes.Any(c => c.ClassID == classId))
                {
                    client.Ismail_Classes.Add(schoolClass);
                    return await _context.SaveChangesAsync() > 0;
                }
            }

            return false;
        }

        public async Task<bool> DeleteAssociationAsync(int userId, int classId)
        {
            var result = await _context.Ismail_Clients
                .Include(c => c.Ismail_Classes)
                .FirstOrDefaultAsync(c => c.UserID == userId && c.Ismail_Classes.Any(cls => cls.ClassID == classId));
            if (result != null)
            {
                var schoolClass = result.Ismail_Classes.FirstOrDefault(cls => cls.ClassID == classId);
                if (schoolClass != null)
                {
                    result.Ismail_Classes.Remove(schoolClass);
                    return await _context.SaveChangesAsync() > 0;
                }
            }
            return false;
        }

        public async Task<bool> ExistsAsync(int userId, int classId)
        {
            return await _context.Ismail_Clients
                .Include(c => c.Ismail_Classes)
                .AnyAsync(c => c.UserID == userId && c.Ismail_Classes.Any(cls => cls.ClassID == classId));
        }

        public async Task<IEnumerable<Ismail_Clients>> GetAssociationsAsync(int? classId, int? userId)
        {
            IQueryable<Ismail_Clients> query = _context.Ismail_Clients
                .Include(c => c.Ismail_Classes)
                .Include(c => c.Ismail_Types);

            if (userId.HasValue)
            {
                query = query.Where(c => c.UserID == userId.Value);
            }
            if (classId.HasValue)
            {
                query = query.Where(c => c.Ismail_Classes.Any(cls => cls.ClassID == classId.Value));
            }
            return await query.ToListAsync();
        }

        public async Task<bool> UpdateAssociationAsync(int userId, int oldClassId, int newClassId)
        {
            var client = await _context.Ismail_Clients
                .Include(c => c.Ismail_Classes)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (client == null)
                return false;

            var oldClass = client.Ismail_Classes.FirstOrDefault(cls => cls.ClassID == oldClassId);
            if (oldClass == null) return false;

            var newClass = await _context.Ismail_Classes.FindAsync(newClassId);
            if (newClass == null) return false;

            client.Ismail_Classes.Remove(oldClass);

            if (!client.Ismail_Classes.Any(c => c.ClassID == newClassId))
            {
                client.Ismail_Classes.Add(newClass);
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
