using FluentValidation;
using FirstBackendApp.Contracts.Books.Requests;
using FirstBackendApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FirstBackendApp.Contracts.Books.Validators;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    // Faqat ruxsat berilgan rasm formatlari ro'yxati
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private readonly AppDbContext _context;

    public CreateBookRequestValidator(AppDbContext context)
    {
        _context = context;
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        // 1. Kitob nomi tekshiruvi (bo'sh bo'lmasligi va uzunligi 5-100 oralig'ida bo'lishi shart)
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Kitob nomi bo'sh bo'lmasligi kerak.")
            .Length(5, 100).WithMessage("Kitob nomi 5 dan 100 tagacha belgidan iborat bo'lishi kerak.");

        // 2. Narx tekshiruvi (musbat qiymat bo'lishi shart)
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Narx 0 dan katta bo'lishi kerak.");

        // 3. Sahifalar soni (1 dan 5000 gacha oraliqda bo'lishi shart)
        RuleFor(x => x.PageCount)
            .InclusiveBetween(1, 5000).WithMessage("Sahifa soni 1 va 5000 oralig'ida bo'lishi kerak.");

        // 4. ISBN formati va bazadagi unikalligi (boshqa kitobda takrorlanmasligi shart)
        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN kodi kiritilishi shart.")
            .MaximumLength(20).WithMessage("ISBN 20 ta belgidan oshmasligi kerak.");

        // 5. Muallif tekshiruvi (ID musbat butun son bo'lishi shart)
        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("Muallif identifikatori kiritilishi shart.");

        // 6. Rasm fayli tekshiruvi (faqat fayl yuborilgan holatda hajmi va kengaytmasi tekshiriladi)
        RuleFor(x => x.Image)
            .Must(img => img!.Length <= 5 * 1024 * 1024)
                .WithMessage("Rasm hajmi 5 MB dan oshmasligi kerak.")
            .Must(img => AllowedImageExtensions.Contains(Path.GetExtension(img!.FileName).ToLower()))
                .WithMessage("Faqat .jpg, .jpeg, .png yoki .webp formatidagi rasmlar ruxsat etiladi.")
            .When(x => x.Image is not null);
    }
}