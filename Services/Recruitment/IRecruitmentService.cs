using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Recruitment;

namespace HR_Management_System.Services.Recruitment
{
    public interface IRecruitmentService
    {
        IReadOnlyList<JobDto> GetJobs();
        JobDto? GetJob(Guid id);
        JobResult CreateJob(JobUpsertRequest request);
        JobResult UpdateJob(Guid id, JobUpsertRequest request);
        JobResult UpdateJobStatus(Guid id, JobStatusRequest request);
        DeleteJobResult DeleteJob(Guid id);
        PagedResponse<CandidateDto> GetCandidates(
         int page,
         int pageSize,
         string? search
     );
        CandidateDto? GetCandidate(Guid id);
        CandidateResult UpdateCandidateStatus(Guid id, CandidateStatusRequest request);
        DeleteCandidateResult DeleteCandidate(Guid id);
    }
}
