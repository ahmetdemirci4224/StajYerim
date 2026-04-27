using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Company
{
    public class AddPostingSkillDto
    {
        [Required(ErrorMessage = "Lütfen bir yetenek (SkillId) belirtin.")]
        public int SkillId { get; set; }

        public bool IsMandatory { get; set; } = true;
    }
}
