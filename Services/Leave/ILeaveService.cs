using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Leaves;

namespace HR_Management_System.Services.Leave
{
    public interface ILeaveService
    {

        PagedResponse<LeaveDto> GetAllLeaves(
        int page,
        int pageSize
    );
        PagedResponse<LeaveDto> GetEmployeeLeaves(
            Guid employeeId,
            int page,
            int pageSize
        );

        PagedResponse<LeaveDto> GetMyLeaves(
        Guid employeeId,
        int page,
        int pageSize
    );
        LeaveResult CreateLeave(
            Guid employeeId,
            CreateLeaveRequest request
        );

        LeaveResult UpdateLeave(
            Guid id,
            UpdateLeaveRequest request,
            Guid employeeId
        );

        DeleteLeaveResult DeleteLeave(
        Guid id,
        Guid employeeId
    );

        LeaveResult UpdateLeaveStatus(
        Guid id,
        UpdateLeaveStatusRequest request
    );
    }

}
