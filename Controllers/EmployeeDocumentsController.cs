using HR_Management_System.Data;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/employees/{employeeId:guid}/documents")]
public class EmployeeDocumentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeeDocumentsController(
        AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDocumentDto>>> GetDocuments(
        Guid employeeId)
    {
        var employeeExists =
            await _context.Employees
                .AnyAsync(x => x.Id == employeeId);

        if (!employeeExists)
        {
            return NotFound("Employee not found.");
        }

        var documents = await _context.EmployeeDocuments
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.UploadedAt)
            .Select(x => new EmployeeDocumentDto(
                x.Id,
                x.DocumentType,
                x.FileName,
                x.FileUrl,
                x.UploadedAt
            ))
            .ToListAsync();

        return Ok(documents);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDocumentDto>> AddDocument(
        Guid employeeId,
        [FromBody] CreateEmployeeDocumentRequest request)
    {
        var employeeExists =
            await _context.Employees
                .AnyAsync(x => x.Id == employeeId);

        if (!employeeExists)
        {
            return NotFound("Employee not found.");
        }

        var document = new EmployeeDocument
        {
            Id = Guid.NewGuid(),

            EmployeeId = employeeId,

            DocumentType =
                request.DocumentType.Trim(),

            FileName =
                request.FileName.Trim(),

            FileUrl =
                request.FileUrl.Trim(),

            UploadedAt = DateTime.UtcNow
        };

        _context.EmployeeDocuments.Add(document);

        await _context.SaveChangesAsync();

        var response = new EmployeeDocumentDto(
            document.Id,
            document.DocumentType,
            document.FileName,
            document.FileUrl,
            document.UploadedAt
        );

        return Ok(response);
    }

    [HttpDelete("{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument(
        Guid employeeId,
        Guid documentId)
    {
        var document =
            await _context.EmployeeDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.EmployeeId == employeeId);

        if (document is null)
        {
            return NotFound("Document not found.");
        }

        _context.EmployeeDocuments.Remove(document);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}