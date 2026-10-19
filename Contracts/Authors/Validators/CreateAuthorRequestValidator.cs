using FluentValidation;
using FirstBackendApp.Contracts.Authors.Requests;

namespace FirstBackendApp.Contracts.Authors.Validators;

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        // 1. Ism tekshiruvi (bo'sh bo'lmasligi va uzunligi 2-100 oralig'ida bo'lishi shart)
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Ism ko‘rsatilishi shart.")
            .Length(2, 100)
            .WithMessage("Ism 2 dan 100 tagacha belgidan iborat bo'lishi kerak.");

        // 2. Familiya tekshiruvi (bo'sh bo'lmasligi va uzunligi 2-100 oralig'ida bo'lishi shart)
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Familiya ko'rsatilishi shart.")
            .Length(2, 100)
            .WithMessage("Familiya 2 dan 100 tagacha belgidan iborat bo'lishi kerak.");

        // 3. Manzil tekshiruvi (faqat manzil kiritilgan holatda uzunligi 5-100 oralig'ida bo'lishi shart)
        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address)
                .Length(5, 100)
                .WithMessage("Manzil 5 dan 100 tagacha belgidan iborat bo'lishi kerak.");
        });
        
        // 4. Tug'ilgan sana tekshiruvi (1800-yil va bugungi kun oralig'ida bo'lishi shart)
        When(x => x.BirthDate is not null, () =>
        {
            RuleFor(x => x.BirthDate)
                .InclusiveBetween(new DateOnly(1800, 1, 1), DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Tug'ilgan sana 1800-yil va bugungi kun oralig'ida bo'lishi kerak.");
        });
    }
}