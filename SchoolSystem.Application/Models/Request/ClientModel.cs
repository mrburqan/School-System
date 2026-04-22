using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Application.Models.Request
{
    public class ClientModel
    {
        public int UserID { get; set; }

        private string _name;
        [Required(ErrorMessage = "Client name is required")]
        [StringLength(50)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        private string _nameAr;
        [Required(ErrorMessage = "الإسم باللغة العربية مطلوب ")]
        [StringLength(50)]
        public string NameAR
        {
            get => _nameAr;
            set => _nameAr = value?.Trim();
        }
        public int LanguageID { get; set; }

        private string _userName;
        [Required(ErrorMessage = "User name is required")]
        [StringLength(50)]
        public string UserName
        {
            get => _userName;
            set => _userName = value?.Trim();
        }

        [Range(1, 2, ErrorMessage = "Please select a gender")]
        public int GenderID { get; set; }

        [Range(1, 3, ErrorMessage = "Please select a type ")]
        public int TypeID { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}