using FluentValidation;
using FirstBackendApp.Contracts.Books.Requests;
using FirstBackendApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FirstBackendApp.Contracts.Books.Validators;

public class PatchBookRequestValidator : AbstractValidator<PatchBookRequest>
{
    // Rasm uchun ruxsat berilgan kengaytmalar ro'yxati
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private readonly AppDbContext _context;

    public PatchBookRequestValidator(AppDbContext context)
    {
        _context = context;
        RuleLevelCascadeMode = CascadeMode.Stop;

        // 1. Kitob nomi (Faqat so'rovda Title yuborilgan bo'lsa tekshiriladi)
        When(x => x.Title is not null, () =>
        {
            RuleFor(x => x.Title!)
                .NotEmpty().WithMessage("Kitob nomi bo'sh bo'lmasligi kerak.")
                .Length(5, 100).WithMessage("Kitob nomi 5 dan 100 tagacha belgidan iborat bo'lishi kerak.");
        });

        // 2. Kitob narxi (Faqat so'rovda Price qiymati mavjud bo'lsa tekshiriladi)
        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price!.Value)
                .GreaterThan(0).WithMessage("Narx 0 dan katta bo'lishi kerak.");
        });

        // 3. Sahifalar soni (Faqat so'rovda PageCount qiymati mavjud bo'lsa tekshiriladi)
        When(x => x.PageCount.HasValue, () =>
        {
            RuleFor(x => x.PageCount!.Value)
                .InclusiveBetween(1, 5000).WithMessage("Sahifa soni 1 va 5000 oralig'ida bo'lishi kerak.");
        });

        // 4. Muallif ID (Faqat so'rovda AuthorId qiymati mavjud bo'lsa tekshiriladi)
        When(x => x.AuthorId.HasValue, () =>
        {
            RuleFor(x => x.AuthorId!.Value)
                .GreaterThan(0).WithMessage("Muallif identifikatori 0 dan katta bo'lishi kerak.");
        });

        // 5. ISBN kodi (Faqat so'rovda Isbn yuborilgan bo'lsa formati va unikalligi tekshiriladi)
        When(x => x.Isbn is not null, () =>
        {
            RuleFor(x => x.Isbn!)
                .NotEmpty().WithMessage("ISBN bo'sh bo'lmasligi kerak.")
                .MaximumLength(20).WithMessage("ISBN 20 ta belgidan oshmasligi kerak.");
        });

        // 6. Rasm fayli (Faqat so'rovda Image fayli biriktirilgan bo'lsa hajmi va kengaytmasi tekshiriladi)
        When(x => x.Image is not null, () =>
        {
            RuleFor(x => x.Image)
                .Must(img => img!.Length > 0)
                    .WithMessage("Yuklangan fayl bo'sh bo'lmasligi kerak.")
                .Must(img => img!.Length <= 5 * 1024 * 1024)
                    .WithMessage("Rasm hajmi 5 MB dan oshmasligi kerak.")
                .Must(img => AllowedImageExtensions.Contains(Path.GetExtension(img!.FileName).ToLower()))
                    .WithMessage("Faqat .jpg, .jpeg, .png yoki .webp formatidagi rasmlar ruxsat etiladi.");
        });
    }
}