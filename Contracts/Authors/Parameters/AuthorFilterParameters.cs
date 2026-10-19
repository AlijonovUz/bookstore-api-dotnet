namespace FirstBackendApp.Contracts.Authors.Parameters;

// Mualliflarni qidirish va sahifalash parametrlari
public class AuthorFilterParameters
{
    // Ism, familiya yoki manzil bo'yicha qidirish
    public string? Search { get; set; }

    // Sahifa raqami
    public int PageNumber { get; set; } = 1;

    // Sahifa hajmi
    public int PageSize { get; set; } = 10;
}