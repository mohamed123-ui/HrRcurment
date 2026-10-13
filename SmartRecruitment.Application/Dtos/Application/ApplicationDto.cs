using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRecruitment.Application.Dtos.Application
{
    public class ApplicationDto
{
    public int Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? MatchScore { get; set; }
}
}
