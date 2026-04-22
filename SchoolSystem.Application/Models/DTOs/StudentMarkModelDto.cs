using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Application.Models.DTOs
{
    public class StudentMarkModelDto
    {
        public int StudentID { get; set; }

        [MaxLength(50, ErrorMessage = "User name cannot exceet 50 characters .")]
        public string UserName { get; set; }

        public List<int?> ClassID { get; set; }
        public List<string> AllClasses { get; set; }

        [Range(0, 100, ErrorMessage = "English mark must be between 0 - 100 ")]
        public int? English { get; set; }

        [Range(0, 100, ErrorMessage = "Math mark must be between 0 - 100 ")]
        public int? Math { get; set; }

        [Range(0, 100, ErrorMessage = "Physics mark must be between 0 - 100 ")]
        public int? Physics { get; set; }
        [Range(0, 100, ErrorMessage = "Chemistry mark must be between 0 - 100 ")]
        public int? Chemistry { get; set; }
        [Range(0, 100, ErrorMessage = "Average must be between 0 and 100")]
        public long? Averge { get; set; }

    }
}
