using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Skill
{
    public class CreateSkillDto
    {
        [Required(ErrorMessage = "Yetenek adı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        public string? Category { get; set; }
    }
}
