namespace HR_Management_System.Entities;

public class LeaveBalance
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public int Year { get; set; }

    public int AnnualAllowance { get; set; }
    public int AnnualUsed { get; set; }

    public int SickAllowance { get; set; }
    public int SickUsed { get; set; }

    public int CasualAllowance { get; set; }
    public int CasualUsed { get; set; }
}