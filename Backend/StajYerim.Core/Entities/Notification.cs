using StajYerim.Core.Common;

namespace StajYerim.Core.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        
        public bool IsRead { get; set; } = false;
        
        public string? Type { get; set; } // "Invitation", "Application"
        public int? RelatedEntityId { get; set; } // InvitationId or ApplicationId
    }
}
