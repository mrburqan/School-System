using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Application.Models.Request
{
    public class StudentMarkModel
    {
        [Required(ErrorMessage = "Student Id is required.")]
        public int StudentID { get; set; }

        [Range(0, 100, ErrorMessage = "English mark must be between 0 - 100 ")]
        public int? English { get; set; }

        [Range(0, 100, ErrorMessage = "Math mark must be between 0 - 100 ")]
        public int? Math { get; set; }

        [Range(0, 100, ErrorMessage = "Physics mark must be between 0 - 100 ")]
        public int? Physics { get; set; }

        [Range(0, 100, ErrorMessage = "Chemistry mark must be between 0 - 100 ")]
        public int? Chemistry { get; set; }

    }
}
