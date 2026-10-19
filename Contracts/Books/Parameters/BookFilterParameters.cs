namespace FirstBackendApp.Contracts.Books.Parameters;

// Kitoblarni qidirish, filtrlash va sahifalash parametrlari
public class BookFilterParameters
{
    // Kitob nomi yoki tavsifi bo'yicha qidiruv
    public string? Search { get; set; }

    // Muallif ID bo'yicha filtrlash
    public int? AuthorId { get; set; }

    // Narx oralig'i (eng past va eng yuqori)
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Sahifa raqami va hajmi
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}