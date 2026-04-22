namespace SchoolSystem.Application.Models.Request
{
    public class AssociationModel
    {
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public int? newClassId { get; set; }
    }
}
