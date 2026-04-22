namespace SchoolSystem.Application.Models
{
    public class AssociationModelDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string TypeDesc { get; set; }
        public int? newClassId { get; set; }
    }
}

