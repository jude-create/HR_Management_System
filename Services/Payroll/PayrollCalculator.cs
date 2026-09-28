using HR_Management_System.Entities.Enums;

using EmployeeEntity = HR_Management_System.Entities.Employee;

namespace HR_Management_System.Services.Payroll;

public static class PayrollCalculator
{
    public static decimal GetBaseCtc(EmployeeEntity employee)
        => employee.Type switch
        {
            EmployeeType.FullTime => 900000m,
            EmployeeType.PartTime => 420000m,
            EmployeeType.Contract => 550000m,
            EmployeeType.Intern => 180000m,
            _ => 400000m
        };
}