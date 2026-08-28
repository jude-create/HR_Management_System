using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Recruitment;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

// RecruitmentService handles jobs and candidates.
public interface IRecruitmentService
{
    IReadOnlyList<JobDto> GetJobs();
    JobDto? GetJob(Guid id);
    JobDto? CreateJob(JobUpsertRequest request);
    JobDto? UpdateJob(Guid id, JobUpsertRequest request);
    JobDto? UpdateJobStatus(Guid id, JobStatusRequest request);
    bool DeleteJob(Guid id);
    IReadOnlyList<CandidateDto> GetCandidates();
    CandidateDto? GetCandidate(Guid id);
    CandidateDto? UpdateCandidateStatus(Guid id, CandidateStatusRequest request);
    bool DeleteCandidate(Guid id);
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

    public JobDto? CreateJob(JobUpsertRequest request)
    {
        // A job must belong to a real department.
        if (!_context.Departments.Any(x => x.Id == request.DepartmentId))
        {
            return null;
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
        return _mapper.Map<JobDto>(job);
    }

    public JobDto? UpdateJob(Guid id, JobUpsertRequest request)
    {
        // Update existing job details if the record and department are valid.
        var job = _context.Jobs.FirstOrDefault(x => x.Id == id);
        if (job is null || !_context.Departments.Any(x => x.Id == request.DepartmentId))
        {
            return null;
        }

        job.Title = request.Title.Trim();
        job.Description = request.Description.Trim();
        job.Roles = request.Roles.Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
        job.Location = request.Location.Trim();
        job.SalaryMin = request.SalaryMin;
        job.SalaryMax = request.SalaryMax;
        job.DepartmentId = request.DepartmentId;
        _context.SaveChanges();
        return _mapper.Map<JobDto>(job);
    }

    public JobDto? UpdateJobStatus(Guid id, JobStatusRequest request)
    {
        // Status is sent as text, so we parse it into the enum.
        var job = _context.Jobs.FirstOrDefault(x => x.Id == id);
        if (job is null || !Enum.TryParse<JobStatus>(request.Status, true, out var status))
        {
            return null;
        }

        job.Status = status;
        _context.SaveChanges();
        return _mapper.Map<JobDto>(job);
    }

    public bool DeleteJob(Guid id)
    {
        // A job cannot be deleted while candidates are still attached to it.
        var job = _context.Jobs.FirstOrDefault(x => x.Id == id);
        if (job is null)
        {
            return false;
        }

        var hasCandidates = _context.Candidates.Any(x => x.JobId == id);
        if (hasCandidates)
        {
            return false;
        }

        _context.Jobs.Remove(job);
        return _context.SaveChanges() > 0;
    }

    public IReadOnlyList<CandidateDto> GetCandidates()
    {
        var candidates = _context.Candidates
            .Include(x => x.Job)
            .AsNoTracking()
            .OrderByDescending(x => x.AppliedDate)
            .ToList();
        return _mapper.Map<List<CandidateDto>>(candidates);
    }

    public CandidateDto? GetCandidate(Guid id)
    {
        var candidate = _context.Candidates
            .Include(x => x.Job)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        return candidate is null ? null : _mapper.Map<CandidateDto>(candidate);
    }

    public CandidateDto? UpdateCandidateStatus(Guid id, CandidateStatusRequest request)
    {
        // Candidate status changes follow the same string-to-enum pattern.
        var candidate = _context.Candidates.FirstOrDefault(x => x.Id == id);
        if (candidate is null || !Enum.TryParse<CandidateStatus>(request.Status, true, out var status))
        {
            return null;
        }

        candidate.Status = status;
        _context.SaveChanges();
        return _mapper.Map<CandidateDto>(candidate);
    }

    public bool DeleteCandidate(Guid id)
    {
        // Candidates can be removed directly if they exist.
        var candidate = _context.Candidates.FirstOrDefault(x => x.Id == id);
        if (candidate is null)
        {
            return false;
        }

        _context.Candidates.Remove(candidate);
        return _context.SaveChanges() > 0;
    }
}
