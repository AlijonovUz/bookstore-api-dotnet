namespace FirstBackendApp.Contracts.Common.Responses;

// Standart API ma'lumotli javob modeli (DTO)
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
    public bool Success { get; set; }

    // Muvaffaqiyatli natija yaratish
    public static ApiResponse<T> SuccessResult(T data) => new()
    {
        Data = data,
        Error = null,
        Success = true
    };

    // Xatolik natijasi yaratish
    public static ApiResponse<T> FailureResult(int statusCode, string message, object? details = null) => new()
    {
        Data = default,
        Error = new ApiError
        {
            ErrorId = statusCode,
            ErrorMsg = message,
            Details = details
        },
        Success = false
    };
}

// Qaytariladigan ma'lumotsiz (masalan DELETE) standart API javob modeli
public class ApiResponse
{
    public bool Success { get; set; }
    public ApiError? Error { get; set; }

    // Muvaffaqiyatli bo'sh natija
    public static ApiResponse SuccessResult() => new()
    {
        Success = true,
        Error = null
    };

    // Xatolik natijasi
    public static ApiResponse FailureResult(int statusCode, string message, object? details = null) => new()
    {
        Success = false,
        Error = new ApiError
        {
            ErrorId = statusCode,
            ErrorMsg = message,
            Details = details
        }
    };
}

// Xatolik tafsiloti modeli
public class ApiError
{
    public int ErrorId { get; set; }
    public string ErrorMsg { get; set; } = string.Empty;
    public object? Details { get; set; }
}