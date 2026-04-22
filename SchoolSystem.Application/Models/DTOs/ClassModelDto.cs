namespace SchoolSystem.Application.Models.DTOs
{
    public class ClassModelDto
    {
        public int? ClassID { get; set; }

        public int? ClassId
        {
            get => ClassID;
            set => ClassID = value;
        }

        private string _className;
        public string ClassName
        {
            get => _className;
            set => _className = value?.Trim();
        }
    }
}
