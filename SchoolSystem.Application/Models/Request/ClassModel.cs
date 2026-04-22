using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolSystem.Application.Models.Request
{
    public class ClassModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassID { get; set; }

        [Required(ErrorMessage = "Class name is required.")]
        [StringLength(50)]
        private string _className;
        public string ClassName
        {
            get => _className;
            set => _className = value?.Trim();
        }
    }
}
