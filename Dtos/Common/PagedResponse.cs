namespace HR_Management_System.Dtos.Common;

public record PagedResponse<T>(IReadOnlyList<T> Data, PageMeta Meta);
