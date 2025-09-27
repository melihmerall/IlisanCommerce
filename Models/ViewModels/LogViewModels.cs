using IlisanCommerce.Models.Logging;

namespace IlisanCommerce.Models.ViewModels
{
    public class SystemLogsViewModel
    {
        public List<SystemLog> Logs { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Level { get; set; }
        public string? Source { get; set; }
        public string? UserId { get; set; }
    }

    public class ActivityLogsViewModel
    {
        public List<ActivityLog> Logs { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public string? UserId { get; set; }
        public bool? IsSuccessful { get; set; }
    }

    public class ErrorLogsViewModel
    {
        public List<ErrorLog> Logs { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Severity { get; set; }
        public string? Category { get; set; }
        public bool? IsResolved { get; set; }
        public string? Search { get; set; }
    }
}
