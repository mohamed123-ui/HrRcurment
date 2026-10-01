using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment.Application.Dtos.Jobs;
using SmartRecruitment.Application.Interfaces;

namespace HrSmartRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _service;
        public JobController(IJobService service)
        {
          _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllJobsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetJobByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.CreateJobAsync(dto, cancellationToken);
            if (result.IsFailure) return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateJobAsync(id, dto, cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
        }

        [HttpPatch("{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken)
        {
            var result = await _service.ToggleJobStatusAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteJobAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
        }
    }
}
