using System.Text.Json.Serialization;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Common.Models;

public class ApiResponse<T> : IApiResponse
{
    /// Indicates whether the API call was successful.
    public bool IsSuccessful { get; set; }

    /// A human-readable message associated with the response.
    public required string Message { get; set; }
    public T? Data { get; set; }



    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Errors { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationMetadata? Pagination { get; set; }



    public static ApiResponse<T> Success(T? data = default, string message = "Request completed successfully.", PaginationMetadata? pagination = null)
    {
        return new ApiResponse<T>
        {
            IsSuccessful = true,
            Message = message,
            Data = data,
            Pagination = pagination

        };
    }



    public static ApiResponse<T> Failure(object? errors = null, string message = "Request failed.")
    {
        return new ApiResponse<T>
        {
            IsSuccessful = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}
