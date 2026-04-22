using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Application.Models.Request
{
    public class GenderModel
    {
        [Range(1, 2, ErrorMessage = "Please select a valid gender ID.")]
        public int GenderID { get; set; }

        private string _genderDesc;
        [Required(ErrorMessage = "Gender description is required.")]
        public string GenderDesc
        {
            get => _genderDesc;
            set => _genderDesc = value?.Trim();
        }
    }
}
