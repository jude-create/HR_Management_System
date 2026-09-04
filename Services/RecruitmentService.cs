using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Recruitment;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// RecruitmentService handles jobs and candidates.
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

// This module owns hiring-related logic and the links between jobs and candidates.
public sealed class RecruitmentService : IRecruitmentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RecruitmentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IReadOnlyList<JobDto> GetJobs()
    {
        var jobs = _context.Jobs
            .Include(x => x.Candidates)
            .AsNoTracking()
            .OrderByDescending(x => x.Status)
            .ToList();
        return _mapper.Map<List<JobDto>>(jobs);
    }

    public JobDto? GetJob(Guid id)
    {
        var job = _context.Jobs
            .Include(x => x.Candidates)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        return job is null ? null : _mapper.Map<JobDto>(job);
    }

    public JobResult CreateJob(JobUpsertRequest request)
    {
        if (request.SalaryMin > request.SalaryMax)
        {
            return JobResult.Fail(JobOperationError.InvalidSalaryRange);
        }

        if (!_context.Departments.Any(x => x.Id == request.DepartmentId))
        {
            return JobResult.Fail(JobOperationError.InvalidDepartment);
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Roles = request.Roles.Select(x => x.Trim()).Where(x => x.Length > 0).ToList(),
            Location = request.Location.Trim(),
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            Status = JobStatus.Draft,
            DepartmentId = request.DepartmentId
        };

        _context.Jobs.Add(job);
        _context.SaveChanges();
        return JobResult.Success(_mapper.Map<JobDto>(job));
    }

    public JobResult UpdateJob(Guid id, JobUpsertRequest request)
    {
        var job = _context.Jobs.FirstOrDefault(x => x.Id == id);
        if (job is null)
        {
            return JobResult.Fail(JobOperationError.NotFound);
        }

        if (request.SalaryMin > request.SalaryMax)
        {
            return JobResult.Fail(JobOperationError.InvalidSalaryRange);
        }

        if (!_context.Departments.Any(x => x.Id == request.DepartmentId))
        {
            return JobResult.Fail(JobOperationError.InvalidDepartment);
        }

        job.Title = request.Title.Trim();
        job.Description = request.Description.Trim();
        job.Roles = request.Roles.Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
        job.Location = request.Location.Trim();
        job.SalaryMin = request.SalaryMin;
        job.SalaryMax = request.SalaryMax;
        job.DepartmentId = request.DepartmentId;

        _context.SaveChanges();
        return JobResult.Success(_mapper.Map<JobDto>(job));
    }

    public JobResult UpdateJobStatus(Guid id, JobStatusRequest request)
    {
        var job = _context.Jobs.FirstOrDefault(x => x.Id == id);
        if (job is null)
        {
            return JobResult.Fail(JobOperationError.NotFound);
        }

        if (!Enum.TryParse<JobStatus>(request.Status, true, out var status))
        {
            return JobResult.Fail(JobOperationError.InvalidStatus);
        }

        job.Status = status;
        _context.SaveChanges();
        return JobResult.Success(_mapper.Map<JobDto>(job));
    }

    public DeleteJobResult DeleteJob(Guid id)
    {
        var job = _context.Jobs.FirstOrDefault(x => x.Id == id);
        if (job is null)
        {
            return DeleteJobResult.Fail(JobOperationError.NotFound);
        }

        var hasCandidates = _context.Candidates.Any(x => x.JobId == id);
        if (hasCandidates)
        {
            return DeleteJobResult.Fail(JobOperationError.HasCandidates);
        }

        _context.Jobs.Remove(job);
        _context.SaveChanges();
        return DeleteJobResult.Ok();
    }

    public PagedResponse<CandidateDto> GetCandidates(
     int page,
     int pageSize,
     string? search)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Candidates
            .Include(x => x.Job)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();

            query = query.Where(x =>
                x.Name.ToLower().Contains(searchTerm) ||
                x.Email.ToLower().Contains(searchTerm) ||
                x.PhoneNumber.ToLower().Contains(searchTerm) ||
                x.Job.Title.ToLower().Contains(searchTerm)
            );
        }

        query = query
            .OrderByDescending(x => x.AppliedDate);

        var totalCount = query.Count();

        var candidates = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var data = _mapper.Map<List<CandidateDto>>(candidates);

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(
                totalCount / (double)pageSize
            );

        return new PagedResponse<CandidateDto>(
            data,
            new PageMeta(
                page,
                pageSize,
                totalCount,
                totalPages
            )
        );
    }

    public CandidateDto? GetCandidate(Guid id)
    {
        var candidate = _context.Candidates
            .Include(x => x.Job)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        return candidate is null ? null : _mapper.Map<CandidateDto>(candidate);
    }

    public CandidateResult UpdateCandidateStatus(Guid id, CandidateStatusRequest request)
    {
        var candidate = _context.Candidates.FirstOrDefault(x => x.Id == id);
        if (candidate is null)
        {
            return CandidateResult.Fail(CandidateOperationError.NotFound);
        }

        if (!Enum.TryParse<CandidateStatus>(request.Status, true, out var status))
        {
            return CandidateResult.Fail(CandidateOperationError.InvalidStatus);
        }

        candidate.Status = status;
        _context.SaveChanges();
        return CandidateResult.Success(_mapper.Map<CandidateDto>(candidate));
    }

    public DeleteCandidateResult DeleteCandidate(Guid id)
    {
        var candidate = _context.Candidates.FirstOrDefault(x => x.Id == id);
        if (candidate is null)
        {
            return DeleteCandidateResult.Fail(CandidateOperationError.NotFound);
        }

        _context.Candidates.Remove(candidate);
        _context.SaveChanges();
        return DeleteCandidateResult.Ok();
    }
}