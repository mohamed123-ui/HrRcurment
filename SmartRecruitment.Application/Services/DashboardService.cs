using SmartRecruitment.Application.Common.Results;
using SmartRecruitment.Application.Dtos.Hr;
using SmartRecruitment.Application.Interfaces;
using SmartRecruitment.Domain.Entities;
using SmartRecruitment.Domain.Hr;
using SmartRecruitment.Domain.Interfaces;
using HrApplication = SmartRecruitment.Domain.Entities.Application;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartRecruitment.Application.Services;

public class DashboardService : IDashboardService
{
    private static readonly string[] StatusOrder =
    {
        HrWorkflow.ApplicationStatuses.Submitted,
        HrWorkflow.ApplicationStatuses.Shortlisted,
        HrWorkflow.ApplicationStatuses.Interview,
        HrWorkflow.ApplicationStatuses.Accepted,
        HrWorkflow.ApplicationStatuses.Rejected
    };

    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultData<DashboardStatsDto>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        // NOTE: Job has no IsActive flag yet, so every job counts as active.
        var jobs = await _unitOfWork.Repository<Job>().GetAllAsync(cancellationToken);
        var applications = await _unitOfWork.Repository<HrApplication>().GetAllAsync(cancellationToken);
        var interviews = await _unitOfWork.Repository<Interview>().GetAllAsync(cancellationToken);

        var upcoming = interviews
            .Where(i => i.Status == HrWorkflow.InterviewStatuses.Scheduled)
            .OrderBy(i => i.ScheduledAt)
            .Take(5)
            .ToList();

        var upcomingDtos = new List<UpcomingInterviewDto>();
        foreach (var interview in upcoming)
        {
            var application = applications.FirstOrDefault(a => a.Id == interview.ApplicationId);
            if (application == null)
                continue;

            var job = jobs.FirstOrDefault(j => j.Id == application.JobId);
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(application.CandidateId, cancellationToken);

            string displayName = $"Candidate #{application.CandidateId}";
            if (candidate != null)
            {
                var link = await _unitOfWork.Repository<CandidateUserLink>()
                    .FirstOrDefaultAsync(l => l.CandidateId == candidate.CandidateId, cancellationToken);
                if (link != null)
                {
                    var user = await _unitOfWork.Repository<User>().GetByIdAsync(link.UserId, cancellationToken);
                    if (user != null && !string.IsNullOrWhiteSpace(user.Name))
                        displayName = user.Name;
                }
            }

            upcomingDtos.Add(new UpcomingInterviewDto
            {
                Id = interview.Id,
                CandidateName = displayName,
                JobTitle = job?.Title ?? string.Empty,
                Type = interview.Type,
                ScheduledAt = interview.ScheduledAt,
                Location = interview.Location
            });
        }

        var stats = new DashboardStatsDto
        {
            ActiveJobs = jobs.Count,
            TotalApplicants = applications.Count,
            ShortlistedCount = applications.Count(a => a.Status == HrWorkflow.ApplicationStatuses.Shortlisted),
            UpcomingInterviewsCount = interviews.Count(i => i.Status == HrWorkflow.InterviewStatuses.Scheduled),
            AcceptedCount = applications.Count(a => a.Status == HrWorkflow.ApplicationStatuses.Accepted),
            RejectedCount = applications.Count(a => a.Status == HrWorkflow.ApplicationStatuses.Rejected),
            UpcomingInterviews = upcomingDtos,
            ApplicantsByStatus = StatusOrder
                .Select(s => new ApplicantsByStatusDto
                {
                    Status = s,
                    Count = applications.Count(a => a.Status == s)
                })
                .ToList()
        };

        return ResultData<DashboardStatsDto>.Success(stats);
    }
}
