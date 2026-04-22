using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Application.Models.Request
{
    public class TypeModel
    {
        [Required(ErrorMessage = "Type ID is required.")]
        public int TypeID { get; set; }

        private string _typeDesc;
        [Required(ErrorMessage = "Type Description is required.")]
        public string TypeDesc
        {
            get => _typeDesc;
            set => _typeDesc = value?.Trim();
        }
    }
}
