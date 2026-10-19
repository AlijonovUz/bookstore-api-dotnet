using System.Text.Json.Serialization;

namespace FirstBackendApp.Contracts.Common.Responses;

// Umumiy sahifalangan javob modeli (DTO)
public class PagedResponse<T>
{
    // Sahifadagi elementlar ro'yxati
    public List<T> Results { get; set; } = [];
    
    [JsonIgnore]
    public int PageNumber { get; set; } = 1;

    [JsonIgnore]
    public int PageSize { get; set; } = 10;
    
    // Umumiy elementlar va sahifalar soni
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    // Oldingi va keyingi sahifalar mavjudligi
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}