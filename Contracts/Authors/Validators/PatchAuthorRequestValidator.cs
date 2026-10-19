using FluentValidation;
using FirstBackendApp.Contracts.Authors.Requests;

namespace FirstBackendApp.Contracts.Authors.Validators;

public class PatchAuthorRequestValidator : AbstractValidator<PatchAuthorRequest>
{
    public PatchAuthorRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        // 1. Ism (faqat so'rovda FirstName yuborilgan bo'lsa tekshiriladi)
        When(x => x.FirstName is not null, () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty()
                    .WithMessage("Ism ko‘rsatilishi shart.")
                    .Length(2, 100)
                    .WithMessage("Ism 2 dan 100 tagacha belgidan iborat bo'lishi kerak.");
            });

        // 2. Familiya (faqat so'rovda LastName yuborilgan bo'lsa tekshiriladi)
        When(x => x.LastName is not null, () =>
            {
                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage("Familiya ko'rsatilishi shart.")
                    .Length(2, 100)
                    .WithMessage("Familiya 2 dan 100 tagacha belgidan iborat bo'lishi kerak.");
            });
        
        // 3. Manzil (faqat so'rovda Address yuborilgan bo'lsa tekshiriladi)
        When(x => x.Address is not null, () =>
            {
                RuleFor(x => x.Address)
                    .Length(5, 100)
                    .WithMessage("Manzil 5 dan 100 tagacha belgidan iborat bo'lishi kerak.");
            });

        // 4. Tug'ilgan sana (faqat so'rovda sana kiritilgan bo'lsa tekshiriladi)
        When(x => x.BirthDate.HasValue, () =>
        {
            RuleFor(x => x.BirthDate)
                .InclusiveBetween(new DateOnly(1800, 1, 1), DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Tug'ilgan sana 1800-yil va bugungi kun oralig'ida bo'lishi kerak.");
        });
    }
}