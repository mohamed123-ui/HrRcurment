using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment.Application.Common.Results;
using SmartRecruitment.Application.Contract;
using SmartRecruitment.Application.Dtos.Candidate;
using SmartRecruitment.Application.Interfaces;
using System.Security.Claims;

namespace HrSmartRecruitment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Candidate")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!);
        }
        // get profole
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var profile = await _candidateService.GetMyProfileAsync(GetCurrentUserId(), cancellationToken);
            if (profile == null)
                return NotFound(ResultData.Failure("No profile linked to this user."));

            return Ok(profile);
        }
        // create
        [HttpPost("profile")]
        public async Task<IActionResult> UpsertProfile([FromBody] UpsertCandidateProfileDto dto, CancellationToken cancellationToken)
        {
            var result = await _candidateService.UpsertMyProfileAsync(GetCurrentUserId(), dto, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("upload-cv")]
        public async Task<IActionResult> UploadCv(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _candidateService.UploadCvAsync(GetCurrentUserId(), file, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("apply-job")]
        public async Task<IActionResult> ApplyJob([FromBody] ApplyJobDto dto, CancellationToken cancellationToken)
        {
            var result = await _candidateService.ApplyForJobAsync(GetCurrentUserId(), dto, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyApplications(CancellationToken cancellationToken)
        {
            var applications = await _candidateService.GetMyApplicationsAsync(GetCurrentUserId(), cancellationToken);
            return Ok(applications);
        }
    }
}