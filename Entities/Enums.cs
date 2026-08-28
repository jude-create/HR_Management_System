namespace HrManagement.Api.Entities;

public enum EmployeeType { FullTime, PartTime, Contract, Intern }
public enum EmployeeStatus { Active, OnLeave, Suspended, Terminated }

public enum AttendanceType { Office, Remote, FieldWork }
public enum AttendanceStatus { Present, Absent, Late, HalfDay, OnLeave }
public enum CorrectionStatus { None, Pending, Approved, Rejected }

public enum PayrollStatus { Draft, Processing, Paid, Failed }

public enum JobStatus { Draft, Open, OnHold, Closed }
public enum CandidateStatus { Applied, Screening, Interview, Offered, Hired, Rejected }

public enum HolidayType { Public, Company, Optional }

public enum NotificationStatus { Unread, Read, Archived }
public enum NotificationActionType { Info, Approval, Alert, Reminder }

public enum UserRole { Admin, HrManager, Employee }
public enum AppearanceMode { Light, Dark, System }
