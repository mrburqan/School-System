using System;

namespace SchoolSystem.Application.Models.DTOs
{
    public class ClientModelDto
    {
        public int UserID { get; set; }

        private string _nameEn;
        public string Name
        {
            get => _nameEn;
            set => _nameEn = value?.Trim();
        }

        private string _nameAr;
        public string NameAR
        {
            get => _nameAr;
            set => _nameAr = value?.Trim();
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => _userName = value?.Trim();
        }

        public int? ClassID { get; set; }

        private string _className;
        public string ClassName
        {
            get => _className;
            set => _className = value?.Trim();
        }

        public int? GenderID { get; set; }
        private string _genderDesc;
        public string GenderDesc
        {
            get => _genderDesc;
            set => _genderDesc = value?.Trim();
        }

        public int? TypeID { get; set; }
        private string _typeDesc;
        public string TypeDesc
        {
            get => _typeDesc;
            set => _typeDesc = value?.Trim();
        }

        public int? LanguageID { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}