using System;
using System.Collections.Generic;

namespace SmartRecruitment.Domain.Entities;

public partial class Hr
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CompanyName { get; set; } = null!;

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual User User { get; set; } = null!;
}
