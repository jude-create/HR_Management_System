using HrManagement.Api.Dtos.Recruitment;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// RecruitmentController groups endpoints for jobs and candidates.
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
        var job = _recruitmentService.CreateJob(request);
        return job is null ? BadRequest("Invalid job payload.") : Ok(job);
    }

    // Updates an existing job posting.
    [HttpPut("jobs/{id:guid}")]
    public ActionResult<JobDto> UpdateJob(Guid id, [FromBody] JobUpsertRequest request)
    {
        var job = _recruitmentService.UpdateJob(id, request);
        return job is null ? NotFound() : Ok(job);
    }

    // Changes the status of a job, such as Draft or Open.
    [HttpPut("jobs/{id:guid}/status")]
    public ActionResult<JobDto> UpdateJobStatus(Guid id, [FromBody] JobStatusRequest request)
    {
        var job = _recruitmentService.UpdateJobStatus(id, request);
        return job is null ? BadRequest("Invalid job status or job not found.") : Ok(job);
    }

    // Deletes a job if no candidates are attached.
    [HttpDelete("jobs/{id:guid}")]
    public IActionResult DeleteJob(Guid id)
    {
        var deleted = _recruitmentService.DeleteJob(id);
        return deleted ? NoContent() : NotFound("Job not found, or candidates are still linked to it.");
    }

    [HttpGet("candidates")]
    public ActionResult<IReadOnlyList<CandidateDto>> GetCandidates()
        => Ok(_recruitmentService.GetCandidates());

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
        var candidate = _recruitmentService.UpdateCandidateStatus(id, request);
        return candidate is null ? BadRequest("Invalid candidate status or candidate not found.") : Ok(candidate);
    }

    // Deletes a candidate.
    [HttpDelete("candidates/{id:guid}")]
    public IActionResult DeleteCandidate(Guid id)
    {
        var deleted = _recruitmentService.DeleteCandidate(id);
        return deleted ? NoContent() : NotFound("Candidate not found.");
    }
}
