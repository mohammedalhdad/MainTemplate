using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Helpers;

public static class ApiResponseFactory
{
    public static ApiResponse<IReadOnlyCollection<T>> SuccessWithPagination<T>(PaginatedList<T> paginatedList, string message = "Data loaded successfully.")
    {
        return ApiResponse<IReadOnlyCollection<T>>.Success(
            data: paginatedList.Items,
            message: message,
            pagination: new PaginationMetadata
            {
                PageNumber = paginatedList.PageNumber,
                TotalPages = paginatedList.TotalPages,
                TotalCount = paginatedList.TotalCount,
                PageSize = paginatedList.PageSize
            }
        );
    }
}
