using FirstBackendApp.Contracts.Books.Parameters;
using FirstBackendApp.Contracts.Books.Requests;
using FirstBackendApp.Contracts.Books.Responses;
using FirstBackendApp.Contracts.Common.Responses;

namespace FirstBackendApp.Services.Books;

// Kitoblar biznes mantiqi uchun shartnoma (Interface)
public interface IBookService
{
    // Barcha kitoblar ro'yxatini olish
    Task<PagedResponse<BookResponse>> GetAllAsync(BookFilterParameters parameters);

    // ID bo'yicha bitta kitobni topish (topilmasa null)
    Task<BookResponse?> GetByIdAsync(int id);

    // Yangi kitob yaratish va uning rasmini yuklash
    Task<BookResponse> CreateAsync(CreateBookRequest request);

    // Mavjud kitobni to'liq yangilash (topilmasa null)
    Task<BookResponse?> UpdateAsync(int id, UpdateBookRequest request);

    // Mavjud kitobni qisman yangilash (topilmasa null)
    Task<BookResponse?> PatchAsync(int id, PatchBookRequest request);

    // Kitobni va uning rasmini o'chirish (muvaffaqiyatli bo'lsa true, topilmasa false)
    Task<bool> DeleteAsync(int id);
}