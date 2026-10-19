using FirstBackendApp.Contracts.Authors.Parameters;
using FirstBackendApp.Contracts.Authors.Requests;
using FirstBackendApp.Contracts.Authors.Responses;
using FirstBackendApp.Contracts.Common.Responses;

namespace FirstBackendApp.Services.Authors;

// Mualliflar biznes mantiqi uchun shartnoma (Interface)
public interface IAuthorService
{
    // Barcha mualliflar ro'yxatini olish
    Task<PagedResponse<AuthorResponse>> GetAllAsync(AuthorFilterParameters parameters);

    // ID bo'yicha bitta muallifni topish (topilmasa null)
    Task<AuthorResponse?> GetByIdAsync(int id);

    // Yangi muallif yaratish
    Task<AuthorResponse> CreateAsync(CreateAuthorRequest request);

    // Mavjud muallifni to'liq yangilash (topilmasa null)
    Task<AuthorResponse?> UpdateAsync(int id, UpdateAuthorRequest request);

    // Mavjud muallifni qisman yangilash (topilmasa null)
    Task<AuthorResponse?> PatchAsync(int id, PatchAuthorRequest request);

    // Muallifni o'chirish (muvaffaqiyatli bo'lsa true, topilmasa false)
    Task<bool> DeleteAsync(int id);
}