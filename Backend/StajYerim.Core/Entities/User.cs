using StajYerim.Core.Common;
using StajYerim.Core.Enums;

namespace StajYerim.Core.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Role Role { get; set; }

        public bool IsEmailVerified { get; set; } = false;
        public string? VerificationCode { get; set; }
        
        // Navigation Properties (1:1 İlişkiler)
        public StudentProfile? StudentProfile { get; set; }
        public CompanyProfile? CompanyProfile { get; set; }
    }
}
