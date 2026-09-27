using System;
using System.Collections.Generic;

namespace SmartRecruitment.Domain.Entities;

public partial class Candidate
{
    public int CandidateId { get; set; }

    public byte Age { get; set; }

    public string EducationLevel { get; set; } = null!;

    public string UniversityTier { get; set; } = null!;

    public double Cgpa { get; set; }

    public byte Internships { get; set; }

    public byte Projects { get; set; }

    public byte ProgrammingLanguages { get; set; }

    public byte Certifications { get; set; }

    public double ExperienceYears { get; set; }

    public byte Hackathons { get; set; }

    public byte ResearchPapers { get; set; }

    public double SkillsScore { get; set; }

    public bool Hired { get; set; }

    public double SoftSkillsScore { get; set; }

    public short ResumeLengthWords { get; set; }

    public string CompanyType { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}
