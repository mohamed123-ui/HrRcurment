using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment.Application.Dtos.Hr;
using SmartRecruitment.Application.Interfaces;

namespace HrSmartRecruitment.Controllers;

/// <summary>
/// HR side — applicants &amp; decisions (Feature 4, Mostafa's part).
/// Reads are open like <see cref="JobController"/> reads; mutations require HR/Admin.
/// </summary>
[Route("api/hr")]
[ApiController]
public class HrApplicantsController : ControllerBase
{
    private readonly IApplicantService _service;

    public HrApplicantsController(IApplicantService service)
    {
        _service = service;
    }

    [HttpGet("jobs/{jobId:int}/applicants")]
    public async Task<IActionResult> GetApplicantsByJob(
        int jobId, [FromQuery] string? search, [FromQuery] string? status, CancellationToken cancellationToken)
    {
        var result = await _service.GetApplicantsByJobAsync(jobId, search, status, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("applications")]
    public async Task<IActionResult> GetShortlisted([FromQuery] string? status, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("Shortlisted", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new[] { "Only status=Shortlisted is supported here." });

        var result = await _service.GetShortlistedCandidatesAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("applications/{id:int}")]
    public async Task<IActionResult> GetApplicationDetails(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetApplicationDetailsAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }

    [HttpPatch("applications/{id:int}/status")]
    [Authorize(Roles = "Admin,HR,Hr,admin,hr")]
    public async Task<IActionResult> UpdateStatus(
        int id, [FromBody] UpdateApplicationStatusDto dto, CancellationToken cancellationToken)
    {
        var changedBy = User.Identity?.Name ?? "HR";
        var result = await _service.UpdateApplicationStatusAsync(id, dto, changedBy, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("applications/{id:int}/cv")]
    public async Task<IActionResult> DownloadCv(int id, CancellationToken cancellationToken)
    {
        var cv = await _service.GetApplicationCvAsync(id, cancellationToken);
        if (cv == null)
            return NotFound(new[] { "No CV attached to this application." });

        return File(cv.Data, cv.ContentType, cv.FileName);
    }
}
