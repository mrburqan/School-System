using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class UtilitiesService : IUtilitiesService
    {
        public Task<string> FirstCharecterToApperCase(string input)
        {
            return FirstCharacterToUpperCase(input);
        }

        public Task<string> FirstCharacterToUpperCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Task.FromResult(input);

            input = input.Trim();

            if (input.Length == 1)
                return Task.FromResult(input.ToUpper());

            var result = char.ToUpper(input[0]) + input.Substring(1).ToLower();

            return Task.FromResult(result);
        }
    }
}