namespace HR_Management_System.Entities;

public class Department
{
    public Guid Id { get; set; }
    public required string Name { get; set; } 
    public string Slug { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
