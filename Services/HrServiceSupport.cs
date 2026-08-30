using System.Security.Cryptography;
using System.Text;
using HR_Management_System.Entities;

namespace HR_Management_System.Services;

// This file is the shared toolbox for the service layer.
// The code here is not a business module by itself; it contains reusable helpers
// that many services need, especially mapping and simple text/enum conversions.
internal static class HrServiceSupport
{
    // Demo-friendly password hashing.
    // This is good enough to show the flow, but a real app should use a stronger
    // password hashing algorithm like BCrypt or Argon2.
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    // Generates a fake token so the login endpoint can return a realistic session object.
    // Later, this can be replaced with JWT or another real auth token.
    public static string CreateToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    // Turns text like "People Operations" into "people-operations" for URLs and display keys.
    public static string Slugify(string value)
        => string.Join("-", value.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    // Payroll calculation uses a simple demo rule based on employee type.
    public static decimal GetBaseCtc(Employee employee)
        => employee.Type switch
        {
            EmployeeType.FullTime => 900000m,
            EmployeeType.PartTime => 420000m,
            EmployeeType.Contract => 550000m,
            EmployeeType.Intern => 180000m,
            _ => 400000m
        };

    // Request bodies send enum values as text, so we need to parse them back into real enums.
    public static bool TryResolveEmployeeType(string value, out EmployeeType employeeType)
        => Enum.TryParse(value, true, out employeeType);

    // Same idea as above, but for the employee status field.
    public static bool TryResolveEmployeeStatus(string value, out EmployeeStatus employeeStatus)
        => Enum.TryParse(value, true, out employeeStatus);
}
