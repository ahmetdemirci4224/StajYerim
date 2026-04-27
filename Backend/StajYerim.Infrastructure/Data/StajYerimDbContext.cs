using Microsoft.EntityFrameworkCore;
using StajYerim.Core.Entities;

namespace StajYerim.Infrastructure.Data
{
    public class StajYerimDbContext : DbContext
    {
        public StajYerimDbContext(DbContextOptions<StajYerimDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<StudentSkill> StudentSkills { get; set; }
        public DbSet<InternshipPosting> InternshipPostings { get; set; }
        public DbSet<PostingSkill> PostingSkills { get; set; }
        public DbSet<JobApplication> Applications { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 1. User
            modelBuilder.Entity<User>(entity => 
            {
                entity.HasIndex(u => u.Email).IsUnique(); // Email benzersiz olmalı
            });

            // 2. StudentProfile (1:1 with User)
            modelBuilder.Entity<StudentProfile>(entity =>
            {
                entity.HasKey(sp => sp.UserId); // PK
                entity.HasOne(sp => sp.User)
                      .WithOne(u => u.StudentProfile)
                      .HasForeignKey<StudentProfile>(sp => sp.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse profili de silinsin
            });

            // 3. CompanyProfile (1:1 with User)
            modelBuilder.Entity<CompanyProfile>(entity =>
            {
                entity.HasKey(cp => cp.UserId); // PK
                entity.HasOne(cp => cp.User)
                      .WithOne(u => u.CompanyProfile)
                      .HasForeignKey<CompanyProfile>(cp => cp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 4. Skill
            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasIndex(s => s.Name).IsUnique(); // Yetenek isimleri benzersiz olmalı (Örn: C# iki kez eklenemez)
            });

            // 5. StudentSkill (Many-to-Many Junction Table)
            modelBuilder.Entity<StudentSkill>(entity =>
            {
                entity.HasKey(ss => new { ss.StudentId, ss.SkillId }); // Composite PK
                
                entity.HasOne(ss => ss.Student)
                      .WithMany(sp => sp.StudentSkills)
                      .HasForeignKey(ss => ss.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(ss => ss.Skill)
                      .WithMany(s => s.StudentSkills)
                      .HasForeignKey(ss => ss.SkillId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 6. PostingSkill (Many-to-Many Junction Table)
            modelBuilder.Entity<PostingSkill>(entity =>
            {
                entity.HasKey(ps => new { ps.PostingId, ps.SkillId }); // Composite PK
                
                entity.HasOne(ps => ps.Posting)
                      .WithMany(ip => ip.PostingSkills)
                      .HasForeignKey(ps => ps.PostingId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(ps => ps.Skill)
                      .WithMany(s => s.PostingSkills)
                      .HasForeignKey(ps => ps.SkillId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 7. Application (Öğrenci Başvurusu)
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasIndex(a => new { a.StudentId, a.PostingId }).IsUnique(); // Bir öğrenci bir ilana sadece 1 kez başvurabilir
                
                entity.HasOne(a => a.Student)
                      .WithMany(sp => sp.Applications)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(a => a.Posting)
                      .WithMany(ip => ip.Applications)
                      .HasForeignKey(a => a.PostingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 8. Invitation (Şirket Daveti)
            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.HasOne(i => i.Company)
                      .WithMany(cp => cp.Invitations)
                      .HasForeignKey(i => i.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(i => i.Student)
                      .WithMany(sp => sp.Invitations)
                      .HasForeignKey(i => i.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(i => i.Posting)
                      .WithMany(ip => ip.Invitations)
                      .HasForeignKey(i => i.PostingId)
                      .OnDelete(DeleteBehavior.SetNull); // İlan silinse bile, davet geçmişi (audit) için null olarak kalabilir
            });
        }
    }
}
