namespace StajYerim.Core.Entities
{
    public class StudentSkill
    {
        public int StudentId { get; set; }
        public StudentProfile Student { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;

        public int ProficiencyLevel { get; set; } // 1: Beginner, 5: Expert
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
