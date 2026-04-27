using StajYerim.Core.Common;

namespace StajYerim.Core.Entities
{
    public class Skill : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        
        // Navigation Properties
        public ICollection<StudentSkill> StudentSkills { get; set; } = new List<StudentSkill>();
        public ICollection<PostingSkill> PostingSkills { get; set; } = new List<PostingSkill>();
    }
}
