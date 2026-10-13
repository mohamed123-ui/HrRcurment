using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRecruitment.Application.Dtos.Candidate
{
    public class UpsertCandidateProfileDto
    {
        public byte Age { get; set; }
        public string EducationLevel { get; set; } = string.Empty;
        public string UniversityTier { get; set; } = string.Empty;
        public double Cgpa { get; set; }
        public byte Internships { get; set; }
        public byte Projects { get; set; }
        public byte ProgrammingLanguages { get; set; }
        public byte Certifications { get; set; }
        public double ExperienceYears { get; set; }
        public byte Hackathons { get; set; }
        public byte ResearchPapers { get; set; }
        public double SkillsScore { get; set; }
        public double SoftSkillsScore { get; set; }
        public short ResumeLengthWords { get; set; }
        public string CompanyType { get; set; } = string.Empty;
        /// <summary>Comma-separated skills, e.g. "Angular, TypeScript" (Feature 4).</summary>
        public string? Skills { get; set; }
        public string? Bio { get; set; }
    }
}
