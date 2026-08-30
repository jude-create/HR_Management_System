using HR_Management_System.Dtos.Recruitment;

namespace HR_Management_System.Dtos.Common;

public enum JobOperationError
{
    None,
    NotFound,
    InvalidDepartment,
    InvalidStatus,
    InvalidSalaryRange,
    HasCandidates
}

public sealed record JobResult(JobDto? Job, JobOperationError Error)
{
    public static JobResult Success(JobDto dto) => new(dto, JobOperationError.None);
    public static JobResult Fail(JobOperationError error) => new(null, error);
}

public enum CandidateOperationError
{
    None,
    NotFound,
    InvalidStatus
}

public sealed record CandidateResult(CandidateDto? Candidate, CandidateOperationError Error)
{
    public static CandidateResult Success(CandidateDto dto) => new(dto, CandidateOperationError.None);
    public static CandidateResult Fail(CandidateOperationError error) => new(null, error);
}

public sealed record DeleteJobResult(bool Success, JobOperationError Error)
{
    public static DeleteJobResult Ok() => new(true, JobOperationError.None);
    public static DeleteJobResult Fail(JobOperationError error) => new(false, error);
}

public sealed record DeleteCandidateResult(bool Success, CandidateOperationError Error)
{
    public static DeleteCandidateResult Ok() => new(true, CandidateOperationError.None);
    public static DeleteCandidateResult Fail(CandidateOperationError error) => new(false, error);
}