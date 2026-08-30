namespace HR_Management_System.Entities;

public class Payroll
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public string Period { get; set; } = string.Empty; // "2026-08"
    public decimal Ctc { get; set; }
    public decimal Salary { get; set; }
    public decimal Deduction { get; set; }
    public PayrollStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
}
