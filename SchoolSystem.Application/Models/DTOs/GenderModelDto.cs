namespace SchoolSystem.Application.Models.DTOs
{
    public class GenderModelDto
    {
        public int? GenderID { get; set; }
        private string _genderDesc;
        public string GenderDesc
        {
            get => _genderDesc;
            set => _genderDesc = value?.Trim();
        }
    }
}
