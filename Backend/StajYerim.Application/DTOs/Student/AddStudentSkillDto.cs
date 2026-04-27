using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Student
{
    public class AddStudentSkillDto
    {
        [Required(ErrorMessage = "Lütfen bir yetenek (SkillId) belirtin.")]
        public int SkillId { get; set; }
        
        [Required(ErrorMessage = "Yetenek seviyesi zorunludur.")]
        [Range(1, 5, ErrorMessage = "Yetenek seviyesi 1 (Başlangıç) ile 5 (Uzman) arasında olmalıdır.")]
        public int ProficiencyLevel { get; set; }
    }
}
