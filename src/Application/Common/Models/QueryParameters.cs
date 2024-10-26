using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.Common.Models;
public abstract record QueryParameters
{
    // Paging
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int PageNumber { get; set; } = 1;

    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    // Sorting
    [MaxLength(50, ErrorMessage = "SortBy length cannot exceed 50 characters.")]
    public string SortBy { get; set; } = "Id";

    public bool IsSortDescending { get; set; } = false;



    // Optional: Adding Filter Parameters (Future Enhancement)
    // public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();

   
}
