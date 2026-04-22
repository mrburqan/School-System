using System.Threading.Tasks;

namespace SchoolSystem.Application.Services.IServices
{
    public interface IUtilitiesService
    {
        Task<string> FirstCharacterToUpperCase(string input);

        Task<string> FirstCharecterToApperCase(string input);
    }
}
