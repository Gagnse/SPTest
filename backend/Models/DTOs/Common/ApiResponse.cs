// backend/Models/DTOs/Common/ApiResponse.cs
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, IEnumerable<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class PaginationRequest
    {
        private int _page = 1;
        private int _pageSize = 10;

        public int Page 
        { 
            get => _page; 
            set => _page = value < 1 ? 1 : value; 
        }

        public int PageSize 
        { 
            get => _pageSize; 
            set => _pageSize = value < 1 ? 10 : value > 100 ? 100 : value; 
        }

        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";
    }
}
