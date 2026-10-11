using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRecruitment.Application.Dtos.Candidate
{
    public class ApplyJobDto
    {
        public int JobId { get; set; }
        public string? CoverLetter { get; set; }
    }
}
