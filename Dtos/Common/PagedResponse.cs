namespace HrManagement.Api.Dtos.Common;

public record PagedResponse<T>(IReadOnlyList<T> Data, PageMeta Meta);
