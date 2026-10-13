using SmartRecruitment.Domain.Entities;
using SmartRecruitment.Domain.Hr;
using SmartRecruitment.Domain.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HrApplication = SmartRecruitment.Domain.Entities.Application;

namespace SmartRecruitment.Infrastructure.Seeder;

/// <summary>
/// Demo graph for Feature 4 (HR: Applicants, Interviews &amp; Dashboard) so the
/// HR UI can run against the real API: one HR user + company, two jobs, three
/// candidates (each with login user + profile link), three applications and one
/// scheduled interview. Fully idempotent — safe to run on every startup.
/// </summary>
public static class HrDemoSeeder
{
    public static async Task SeedDemoGraphAsync(IUnitOfWork unitOfWork)
    {
        var existingJobs = await unitOfWork.Repository<Job>().GetAllAsync();
        if (existingJobs.Any())
            return;

        var hrUser = await unitOfWork.Repository<User>()
            .FirstOrDefaultAsync(u => u.Email == "hr.demo@smartrecruitment.com");
        if (hrUser == null)
        {
            hrUser = new User
            {
                Name = "Demo HR",
                Email = "hr.demo@smartrecruitment.com",
                PasswordHash = "DEMO-NO-LOGIN",
                Role = "HR",
                CreatedAt = DateTime.UtcNow
            };
            await unitOfWork.Repository<User>().AddAsync(hrUser);
            await unitOfWork.SaveChangesAsync();
        }

        var hr = (await unitOfWork.Repository<Hr>()
                .FindAsync(h => h.UserId == hrUser.Id))
            .FirstOrDefault();
        if (hr == null)
        {
            hr = new Hr { UserId = hrUser.Id, CompanyName = "Demo Company" };
            await unitOfWork.Repository<Hr>().AddAsync(hr);
            await unitOfWork.SaveChangesAsync();
        }

        var frontendJob = new Job
        {
            Hrid = hr.Id,
            Title = "Front-End Developer (Angular)",
            Description = "Build enterprise Angular applications.",
            RequiredSkills = "Angular, TypeScript, RxJS, SCSS",
            ExperienceRequired = 2,
            CreatedAt = DateTime.UtcNow
        };
        var backendJob = new Job
        {
            Hrid = hr.Id,
            Title = "Backend Developer (.NET)",
            Description = "Build .NET Core APIs with Clean Architecture.",
            RequiredSkills = ".NET Core, C#, EF Core, SQL Server",
            ExperienceRequired = 3,
            CreatedAt = DateTime.UtcNow
        };
        await unitOfWork.Repository<Job>().AddAsync(frontendJob);
        await unitOfWork.Repository<Job>().AddAsync(backendJob);
        await unitOfWork.SaveChangesAsync();

        var ahmed = await AddCandidateAsync(unitOfWork,
            name: "Ahmed Samy Abdelaziz",
            email: "ahmed.samy@mail.com",
            skills: "Angular, TypeScript, RxJS, SCSS, Git",
            years: 4,
            education: "Bachelor",
            bio: "Front-End developer specialized in Angular.");

        var mona = await AddCandidateAsync(unitOfWork,
            name: "Mona Abdelrahman Khalil",
            email: "mona.abdelrahman@mail.com",
            skills: "Angular, TypeScript, Tailwind, Jest, REST",
            years: 3,
            education: "Bachelor",
            bio: "Front-End developer focused on performance and user experience.");

        var sara = await AddCandidateAsync(unitOfWork,
            name: "Sara Ibrahim Mansour",
            email: "sara.ibrahim@mail.com",
            skills: ".NET Core, C#, EF Core, SQL Server, Docker",
            years: 5,
            education: "Bachelor",
            bio: "Backend developer with 5 years of .NET Core experience.");

        await AddApplicationAsync(unitOfWork, ahmed, frontendJob.Id,
            HrWorkflow.ApplicationStatuses.Shortlisted, 92,
            mlFeedback: "Very high match (92%). Strongest points: matching hands-on experience (4 years).",
            note: "Excellent Angular experience, strong CV.", changedBy: "Mostafa (HR)");

        await AddApplicationAsync(unitOfWork, mona, frontendJob.Id,
            "Applied", 88,
            mlFeedback: "Strong match (88%). RxJS should be assessed in the technical interview.",
            note: null, changedBy: null);

        var saraApp = await AddApplicationAsync(unitOfWork, sara, backendJob.Id,
            HrWorkflow.ApplicationStatuses.Interview, 95,
            mlFeedback: "Excellent match (95%). 5 years of .NET Core with Clean Architecture.",
            note: null, changedBy: null);

        await unitOfWork.Repository<Interview>().AddAsync(new Interview
        {
            ApplicationId = saraApp.Id,
            Type = HrWorkflow.InterviewTypes.Technical,
            ScheduledAt = DateTime.UtcNow.AddDays(2),
            Location = "Online - Microsoft Teams",
            Status = HrWorkflow.InterviewStatuses.Scheduled,
            CreatedBy = "Mahmoud (HR)",
            CreatedAt = DateTime.UtcNow
        });
        await unitOfWork.SaveChangesAsync();
    }

    private static async Task<Candidate> AddCandidateAsync(
        IUnitOfWork unitOfWork, string name, string email,
        string skills, double years, string education, string bio)
    {
        var user = await unitOfWork.Repository<User>()
            .FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = "DEMO-NO-LOGIN",
                Role = "Candidate",
                CreatedAt = DateTime.UtcNow
            };
            await unitOfWork.Repository<User>().AddAsync(user);
            await unitOfWork.SaveChangesAsync();
        }

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fileName = name.Replace(' ', '-') + "-CV.pdf";
        var filePath = Path.Combine(folderPath, fileName);
        if (!File.Exists(filePath))
            await File.WriteAllTextAsync(filePath, $"Demo CV of {name}.");

        var candidate = new Candidate
        {
            Age = 25,
            EducationLevel = education,
            UniversityTier = "Tier 1",
            ExperienceYears = years,
            CompanyType = "Product",
            Skills = skills,
            Bio = bio,
            CvFileName = fileName,
            CvContentType = "application/pdf",
            CvSizeBytes = new FileInfo(filePath).Length,
            CvFilePath = filePath
        };
        await unitOfWork.Repository<Candidate>().AddAsync(candidate);
        await unitOfWork.SaveChangesAsync();

        await unitOfWork.Repository<CandidateUserLink>().AddAsync(new CandidateUserLink
        {
            UserId = user.Id,
            CandidateId = candidate.CandidateId
        });
        await unitOfWork.SaveChangesAsync();

        return candidate;
    }

    private static async Task<HrApplication> AddApplicationAsync(
        IUnitOfWork unitOfWork, Candidate candidate, int jobId,
        string status, decimal matchScore, string mlFeedback,
        string? note, string? changedBy)
    {
        var application = new HrApplication
        {
            CandidateId = candidate.CandidateId,
            JobId = jobId,
            AppliedAt = DateTime.UtcNow.AddDays(-5),
            Status = status,
            MatchScore = matchScore,
            MlFeedback = mlFeedback
        };
        await unitOfWork.Repository<HrApplication>().AddAsync(application);
        await unitOfWork.SaveChangesAsync();

        await unitOfWork.Repository<ApplicationStatusHistory>().AddAsync(new ApplicationStatusHistory
        {
            ApplicationId = application.Id,
            Status = HrWorkflow.ApplicationStatuses.Submitted,
            ChangedAt = application.AppliedAt,
            ChangedBy = "System"
        });

        if (status != HrWorkflow.ApplicationStatuses.Submitted)
        {
            await unitOfWork.Repository<ApplicationStatusHistory>().AddAsync(new ApplicationStatusHistory
            {
                ApplicationId = application.Id,
                Status = status,
                ChangedAt = DateTime.UtcNow.AddDays(-1),
                ChangedBy = changedBy ?? "HR",
                Note = note
            });
        }

        await unitOfWork.SaveChangesAsync();
        return application;
    }
}
