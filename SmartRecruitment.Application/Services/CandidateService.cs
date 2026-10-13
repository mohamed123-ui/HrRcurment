using FluentValidation;
using Microsoft.AspNetCore.Http;
using SmartRecruitment.Application.Common.Results;
using SmartRecruitment.Application.Contract;
using SmartRecruitment.Application.Dtos.Application;
using SmartRecruitment.Application.Dtos.Candidate;
using SmartRecruitment.Domain.Entities;
using SmartRecruitment.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRecruitment.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpsertCandidateProfileDto> _profileValidator;

        public CandidateService(IUnitOfWork unitOfWork, IValidator<UpsertCandidateProfileDto> profileValidator)
        {
            _unitOfWork = unitOfWork;
            _profileValidator = profileValidator;
        }
        public async Task<ResultData> ApplyForJobAsync(int userId, ApplyJobDto dto, CancellationToken cancellationToken)
        {  
            var link= await _unitOfWork.Repository<CandidateUserLink>()
                            .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);
            if (link == null)
                return ResultData.Failure("Please complete your profile before applying.");
            var job = await _unitOfWork.Repository<Job>()
                .GetByIdAsync(dto.JobId, cancellationToken);
            if (job == null)
                return ResultData.Failure("Job not found.");

            var existingApp = await _unitOfWork.Repository<Domain.Entities.Application>()
                .FirstOrDefaultAsync(a => a.JobId == dto.JobId && a.CandidateId == link.CandidateId, cancellationToken);

            if (existingApp != null)
                return ResultData.Failure("You have already applied for this job.");
            var application = new Domain.Entities.Application
            {
                CandidateId = link.CandidateId,
                JobId = dto.JobId,
                AppliedAt = DateTime.UtcNow,
                Status = "Applied",
                MatchScore = null // This can be calculated later based on the candidate's profile and job requirements
            };
            await _unitOfWork.Repository<Domain.Entities.Application>().AddAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ResultData.Success();
        }

        public async Task<IEnumerable<ApplicationDto>> GetMyApplicationsAsync(int userId, CancellationToken cancellationToken)
        {
            var link = await _unitOfWork.Repository<CandidateUserLink>()
          .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (link == null)
                return Enumerable.Empty<ApplicationDto>();

        
            var applications = await _unitOfWork.Repository<SmartRecruitment.Domain.Entities.Application>()
                .FindAsync(x => x.CandidateId == link.CandidateId, cancellationToken);

            var applicationDtos = new List<ApplicationDto>();

            foreach (var app in applications)
            {
                var job = await _unitOfWork.Repository<SmartRecruitment.Domain.Entities.Job>()
                    .GetByIdAsync(app.JobId, cancellationToken);

                applicationDtos.Add(new ApplicationDto
                {
                    Id = app.Id,
                    JobTitle = job?.Title ?? string.Empty,
                    AppliedAt = app.AppliedAt,
                    Status = app.Status,
                    MatchScore = app.MatchScore ?? 0m
                });
            }

            return applicationDtos;
        }

        public async Task<ResultData> GetMyProfileAsync(int userId, CancellationToken cancellationToken)
        {

            var link = await _unitOfWork.Repository<CandidateUserLink>()
                            .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);

            if (link == null)
                return ResultData.Failure("No profile linked to this user yet.");

            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(link.CandidateId, cancellationToken);

            var user = await _unitOfWork.Repository<User>()
                .GetByIdAsync(userId, cancellationToken);

            if (candidate == null)
                return ResultData.Failure("Candidate profile record missing.");

            var profileDto = new CandidateProfileDto
            {
                CandidateId = candidate.CandidateId,
                Name = user?.Name ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Age = candidate.Age,
                EducationLevel = candidate.EducationLevel,
                UniversityTier = candidate.UniversityTier,
                Cgpa = candidate.Cgpa,
                Internships = candidate.Internships,
                Projects = candidate.Projects,
                ProgrammingLanguages = candidate.ProgrammingLanguages,
                Certifications = candidate.Certifications,
                ExperienceYears = candidate.ExperienceYears,
                Hackathons = candidate.Hackathons,
                ResearchPapers = candidate.ResearchPapers,
                SkillsScore = candidate.SkillsScore,
                Hired = candidate.Hired,
                SoftSkillsScore = candidate.SoftSkillsScore,
                ResumeLengthWords = candidate.ResumeLengthWords,
                CompanyType = candidate.CompanyType
            };

            return ResultData<CandidateProfileDto>.Success(profileDto);
        }

        public async Task<ResultData> UploadCvAsync(int userId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return ResultData.Failure("Invalid file.");

            var link = await  _unitOfWork.Repository<CandidateUserLink>()
                .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);

            if (link == null)
                return ResultData.Failure("Please create your candidate profile first.");

            var candidate = await _unitOfWork.Repository<Candidate>().GetByIdAsync(link.CandidateId, cancellationToken);
            if (candidate == null)
                return ResultData.Failure("Candidate profile not found.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            // Feature 4: remember where the CV lives so the HR side can preview/download it.
            candidate.CvFileName = file.FileName;
            candidate.CvContentType = file.ContentType;
            candidate.CvSizeBytes = file.Length;
            candidate.CvFilePath = filePath;
            _unitOfWork.Repository<Candidate>().Update(candidate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResultData.Success();
        }

        public async Task<ResultData> UpsertMyProfileAsync(int userId, UpsertCandidateProfileDto dto, CancellationToken cancellationToken)
        {
            // Validate the DTO
            var validationResult = await _profileValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ResultData.Failure(errors);
            }
            var link = await _unitOfWork.Repository<CandidateUserLink>()
                            .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);
            if (link == null)
            {
                 var candidate = new Candidate
                {
                    Age = dto.Age,
                    EducationLevel = dto.EducationLevel,
                    UniversityTier = dto.UniversityTier,
                    Cgpa = dto.Cgpa,
                    Internships = dto.Internships,
                    Projects = dto.Projects,
                    ProgrammingLanguages = dto.ProgrammingLanguages,
                    Certifications = dto.Certifications,
                    ExperienceYears = dto.ExperienceYears,
                    Hackathons = dto.Hackathons,
                    ResearchPapers = dto.ResearchPapers,
                    SkillsScore = dto.SkillsScore,
                    SoftSkillsScore = dto.SoftSkillsScore,
                    ResumeLengthWords = dto.ResumeLengthWords,
                    CompanyType = dto.CompanyType,
                    Skills = dto.Skills,
                    Bio = dto.Bio
                };
                await _unitOfWork.Repository<Candidate>().AddAsync(candidate, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                var newLink = new CandidateUserLink
                {
                    UserId = userId,
                    CandidateId = candidate.CandidateId
                };
                await _unitOfWork.Repository<CandidateUserLink>().AddAsync(newLink, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
                else
                {
                  var  candidate = await _unitOfWork.Repository<Candidate>().GetByIdAsync(link.CandidateId, cancellationToken);
                    if (candidate == null)
                        return ResultData.Failure("Linked candidate record not found.");

                    candidate.Age = dto.Age;
                    candidate.EducationLevel = dto.EducationLevel;
                    candidate.UniversityTier = dto.UniversityTier;
                    candidate.Cgpa = dto.Cgpa;
                    candidate.Internships = dto.Internships;
                    candidate.Projects = dto.Projects;
                    candidate.ProgrammingLanguages = dto.ProgrammingLanguages;
                    candidate.Certifications = dto.Certifications;
                    candidate.ExperienceYears = dto.ExperienceYears;
                    candidate.Hackathons = dto.Hackathons;
                    candidate.ResearchPapers = dto.ResearchPapers;
                    candidate.SkillsScore = dto.SkillsScore;
                    candidate.SoftSkillsScore = dto.SoftSkillsScore;
                    candidate.ResumeLengthWords = dto.ResumeLengthWords;
                    candidate.CompanyType = dto.CompanyType;
                    candidate.Skills = dto.Skills;
                    candidate.Bio = dto.Bio;

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return ResultData.Success();
            }
        }
    }
