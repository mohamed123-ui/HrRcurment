using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRecruitment.Domain.Entities
{
    [Table("Candidates")]
    public partial class Candidate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Column("candidate_id")]
        public int CandidateId { get; set; }

        [Column("age")]
        public byte Age { get; set; }

        [Column("education_level")]
        public string EducationLevel { get; set; } = null!;

        [Column("university_tier")]
        public string UniversityTier { get; set; } = null!;

        [Column("cgpa")]
        public double Cgpa { get; set; }

        [Column("internships")]
        public byte Internships { get; set; }

        [Column("projects")]
        public byte Projects { get; set; }

        [Column("programming_languages")]
        public byte ProgrammingLanguages { get; set; }

        [Column("certifications")]
        public byte Certifications { get; set; }

        [Column("experience_years")]
        public double ExperienceYears { get; set; }

        [Column("hackathons")]
        public byte Hackathons { get; set; }

        [Column("research_papers")]
        public byte ResearchPapers { get; set; }

        [Column("skills_score")]
        public double SkillsScore { get; set; }

        [Column("hired")]
        public bool Hired { get; set; }

        [Column("soft_skills_score")]
        public double SoftSkillsScore { get; set; }

        [Column("resume_length_words")]
        public short ResumeLengthWords { get; set; }

        [Column("company_type")]
        public string CompanyType { get; set; } = null!;

        // ---- Feature 4 (HR side) additive fields ----
        // Human profile data used by the HR applicants screens.
        // Identity (name/email) stays on the linked User (Feature 3 model).

        [Column("skills")]
        public string? Skills { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("cv_file_name")]
        public string? CvFileName { get; set; }

        [Column("cv_content_type")]
        public string? CvContentType { get; set; }

        [Column("cv_size_bytes")]
        public long? CvSizeBytes { get; set; }

        [Column("cv_file_path")]
        public string? CvFilePath { get; set; }

        public CandidateUserLink? UserLink { get; set; }

        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}