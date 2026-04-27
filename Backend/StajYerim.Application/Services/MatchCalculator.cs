using StajYerim.Core.Entities;

namespace StajYerim.Application.Services
{
    public static class MatchCalculator
    {
        public static double CalculateMatchScore(
            InternshipPosting posting, 
            StudentProfile studentProfile)
        {
            // Eğer profil boşsa %0 döner
            if (studentProfile == null || posting == null) return 0;

            double partA_SkillMatchScore = CalculateSkillMatchScore(posting.PostingSkills, studentProfile.StudentSkills, studentProfile.Gpa);
            double partB_PreferenceMatchScore = CalculatePreferenceMatchScore(posting, studentProfile);

            // Ağırlıklı Ortalama: Yetenekler %60, Beklentiler %40
            double finalScore = (partA_SkillMatchScore * 0.6) + (partB_PreferenceMatchScore * 0.4);

            return Math.Min(100.00, Math.Round(finalScore, 2));
        }

        // Geriye dönük uyumluluk (Eski servisler kırılmasın diye)
        public static double CalculateMatchScore(
            ICollection<PostingSkill> postingSkills, 
            ICollection<StudentSkill> studentSkills, 
            decimal? studentGpa)
        {
            return CalculateSkillMatchScore(postingSkills, studentSkills, studentGpa);
        }

        private static double CalculateSkillMatchScore(
            ICollection<PostingSkill> postingSkills, 
            ICollection<StudentSkill> studentSkills, 
            decimal? studentGpa)
        {
            if (postingSkills == null || !postingSkills.Any()) return 100;

            double totalPoints = 0;
            double maxPossiblePoints = 0;

            double mandatoryWeight = 3.0; 
            double optionalWeight = 1.0;  
            double levelPenalty = 0.2;    

            var studentSkillsDict = studentSkills?.ToDictionary(s => s.SkillId, s => s.ProficiencyLevel) ?? new Dictionary<int, int>();

            foreach (var ps in postingSkills)
            {
                double weight = ps.IsMandatory ? mandatoryWeight : optionalWeight;
                maxPossiblePoints += weight;

                if (studentSkillsDict.TryGetValue(ps.SkillId, out int studentLevel))
                {
                    int requiredLevel = 3; 
                    
                    int levelDiff = Math.Max(0, requiredLevel - studentLevel);
                    double penalty = levelDiff * levelPenalty;
                    double baseScore = Math.Max(0, 1.0 - penalty);
                    
                    totalPoints += baseScore * weight;
                }
            }

            if (maxPossiblePoints == 0) return 0;
            
            double matchPercentage = (totalPoints / maxPossiblePoints) * 100;

            if (studentGpa.HasValue)
            {
                double gpaBonus = ((double)studentGpa.Value / 4.0) * 10.0;
                matchPercentage += gpaBonus;
            }

            return Math.Min(100.00, Math.Round(matchPercentage, 2));
        }

        private static double CalculatePreferenceMatchScore(InternshipPosting posting, StudentProfile student)
        {
            double score = 0;
            double maxScore = 3.0; // 3 Criteria: Location, WorkModel, Roles

            // 1. Lokasyon Uyum (Location Match)
            if (string.IsNullOrEmpty(student.PreferredLocation) || student.PreferredLocation.Equals("Fark Etmez", StringComparison.OrdinalIgnoreCase))
            {
                score += 1.0;
            }
            else if (!string.IsNullOrEmpty(posting.Location) && posting.Location.Contains(student.PreferredLocation, StringComparison.OrdinalIgnoreCase))
            {
                score += 1.0;
            }

            // 2. Çalışma Modeli Uyum (WorkModel Match)
            if (string.IsNullOrEmpty(student.PreferredWorkModel) || student.PreferredWorkModel.Equals("Fark Etmez", StringComparison.OrdinalIgnoreCase))
            {
                score += 1.0;
            }
            else if (!string.IsNullOrEmpty(posting.WorkModel) && posting.WorkModel.Equals(student.PreferredWorkModel, StringComparison.OrdinalIgnoreCase))
            {
                score += 1.0;
            }

            // 3. Rol Uyum (Role Match)
            if (string.IsNullOrEmpty(student.InterestedRoles))
            {
                score += 1.0;
            }
            else
            {
                var roles = student.InterestedRoles.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim().ToLower());
                bool roleMatch = false;
                foreach (var role in roles)
                {
                    if (posting.Title.ToLower().Contains(role))
                    {
                        roleMatch = true;
                        break;
                    }
                }
                if (roleMatch) score += 1.0;
            }

            return (score / maxScore) * 100;
        }
    }
}
