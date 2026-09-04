using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Recruitment;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// RecruitmentController groups endpoints for jobs and candidates.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecruitmentController : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService;

    public RecruitmentController(IRecruitmentService recruitmentService)
    {
        _recruitmentService = recruitmentService;
    }

    // Returns all jobs.
    [HttpGet("jobs")]
    public ActionResult<IReadOnlyList<JobDto>> GetJobs()
        => Ok(_recruitmentService.GetJobs());

    // Returns one job.
    [HttpGet("jobs/{id:guid}")]
    public ActionResult<JobDto> GetJob(Guid id)
    {
        var job = _recruitmentService.GetJob(id);
        return job is null ? NotFound() : Ok(job);
    }

    // Creates a new job posting.
    [HttpPost("jobs")]
    public ActionResult<JobDto> CreateJob([FromBody] JobUpsertRequest request)
    {
        var result = _recruitmentService.CreateJob(request);

        return result.Error switch
        {
            JobOperationError.None => Ok(result.Job),
            JobOperationError.InvalidDepartment => BadRequest("Department does not exist."),
            JobOperationError.InvalidSalaryRange => BadRequest("SalaryMin cannot be greater than SalaryMax."),
            _ => BadRequest("Invalid job payload.")
        };
    }

    // Updates an existing job posting.
    [HttpPut("jobs/{id:guid}")]
    public ActionResult<JobDto> UpdateJob(Guid id, [FromBody] JobUpsertRequest request)
    {
        var result = _recruitmentService.UpdateJob(id, request);

        return result.Error switch
        {
            JobOperationError.None => Ok(result.Job),
            JobOperationError.NotFound => NotFound("Job not found."),
            JobOperationError.InvalidDepartment => BadRequest("Department does not exist."),
            JobOperationError.InvalidSalaryRange => BadRequest("SalaryMin cannot be greater than SalaryMax."),
            _ => BadRequest("Invalid job payload.")
        };
    }

    // Changes the status of a job, such as Draft or Open.
    [HttpPut("jobs/{id:guid}/status")]
    public ActionResult<JobDto> UpdateJobStatus(Guid id, [FromBody] JobStatusRequest request)
    {
        var result = _recruitmentService.UpdateJobStatus(id, request);

        return result.Error switch
        {
            JobOperationError.None => Ok(result.Job),
            JobOperationError.NotFound => NotFound("Job not found."),
            JobOperationError.InvalidStatus => BadRequest("Invalid job status."),
            _ => BadRequest("Invalid job status or job not found.")
        };
    }

    // Deletes a job if no candidates are attached.
    [HttpDelete("jobs/{id:guid}")]
    public IActionResult DeleteJob(Guid id)
    {
        var result = _recruitmentService.DeleteJob(id);

        return result.Error switch
        {
            JobOperationError.None => NoContent(),
            JobOperationError.NotFound => NotFound("Job not found."),
            JobOperationError.HasCandidates => Conflict("Candidates are still linked to this job."),
            _ => BadRequest("Unable to delete job.")
        };
    }

    [HttpGet("candidates")]
    public ActionResult<PagedResponse<CandidateDto>> GetCandidates(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null)
    {
        var result = _recruitmentService.GetCandidates(
            page,
            pageSize,
            search
        );

        return Ok(result);
    }
    // Returns one candidate.
    [HttpGet("candidates/{id:guid}")]
    public ActionResult<CandidateDto> GetCandidate(Guid id)
    {
        var candidate = _recruitmentService.GetCandidate(id);
        return candidate is null ? NotFound() : Ok(candidate);
    }

    // Updates the candidate's pipeline status.
    [HttpPut("candidates/{id:guid}/status")]
    public ActionResult<CandidateDto> UpdateCandidateStatus(Guid id, [FromBody] CandidateStatusRequest request)
    {
        var result = _recruitmentService.UpdateCandidateStatus(id, request);

        return result.Error switch
        {
            CandidateOperationError.None => Ok(result.Candidate),
            CandidateOperationError.NotFound => NotFound("Candidate not found."),
            CandidateOperationError.InvalidStatus => BadRequest("Invalid candidate status."),
            _ => BadRequest("Invalid candidate status or candidate not found.")
        };
    }

    // Deletes a candidate.
    [Authorize(Roles = "Admin")]
    [HttpDelete("candidates/{id:guid}")]
    public IActionResult DeleteCandidate(Guid id)
    {
        var result = _recruitmentService.DeleteCandidate(id);

        return result.Error switch
        {
            CandidateOperationError.None => NoContent(),
            CandidateOperationError.NotFound => NotFound("Candidate not found."),
            _ => BadRequest("Unable to delete candidate.")
        };
    }
}