namespace StajYerim.Core.Entities
{
    public class PostingSkill
    {
        public int PostingId { get; set; }
        public InternshipPosting Posting { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;

        public bool IsMandatory { get; set; } = true;
    }
}
