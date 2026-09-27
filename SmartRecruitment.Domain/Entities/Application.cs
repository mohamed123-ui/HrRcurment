using System;
using System.Collections.Generic;

namespace SmartRecruitment.Domain.Entities;

public partial class Application
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public int JobId { get; set; }

    public DateTime AppliedAt { get; set; }

    public string Status { get; set; } = null!;

    public decimal? MatchScore { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;
}
