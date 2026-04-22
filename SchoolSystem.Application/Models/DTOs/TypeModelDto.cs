namespace SchoolSystem.Application.Models.DTOs
{
    public class TypeModelDto
    {
        public int? TypeID { get; set; }
        private string _typeDesc;
        public string TypeDesc
        {
            get => _typeDesc;
            set => _typeDesc = value?.Trim();
        }
    }
}

