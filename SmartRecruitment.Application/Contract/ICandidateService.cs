using Microsoft.AspNetCore.Http;
using SmartRecruitment.Application.Common.Results;
using SmartRecruitment.Application.Dtos.Candidate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRecruitment.Application.Contract
{
    public interface ICandidateService
    {
        Task<ResultData> GetMyProfileAsync(int userId, CancellationToken cancellationToken);
        Task<ResultData> UpsertMyProfileAsync(int userId, UpsertCandidateProfileDto dto, CancellationToken cancellationToken);
        Task<ResultData> UploadCvAsync(int userId, IFormFile file, CancellationToken cancellationToken);
        Task<ResultData> ApplyForJobAsync(int userId, ApplyJobDto dto, CancellationToken cancellationToken);
    }
}
