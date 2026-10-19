using FluentValidation;
using FirstBackendApp.Contracts.Auth.Requests;

namespace FirstBackendApp.Contracts.Auth.Validators;

// Ro'yxatdan o'tish so'rovini tekshiruvchi validator
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        // Username: 4-32 belgidan iborat bo'lishi shart
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Foydalanuvchi nomi kiritilishi shart.")
            .MinimumLength(4).WithMessage("Foydalanuvchi nomi kamida 4 ta belgidan iborat bo'lishi kerak.")
            .MaximumLength(32).WithMessage("Foydalanuvchi nomi 32 ta belgidan oshmasligi kerak.");

        // Parol: kiritilishi shart
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol kiritilishi shart.");

        // Email: kiritilgan bo'lsa email formatida bo'lishi shart
        When(x => x.Email is not null, () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Elektron pochta manzili noto'g'ri formatda kiritildi.");
        });

        // Ism: kiritilgan bo'lsa kamida 3 belgi bo'lishi shart
        When(x => x.FirstName is not null, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ism bo'sh bo'lmasligi kerak.")
                .MinimumLength(3).WithMessage("Ism kamida 3 ta belgidan iborat bo'lishi kerak.");
        });
        
        // Familiya: kiritilgan bo'lsa kamida 5 belgi bo'lishi shart
        When(x => x.LastName is not null, () =>
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Familiya bo'sh bo'lmasligi kerak.")
                .MinimumLength(5).WithMessage("Familiya kamida 5 ta belgidan iborat bo'lishi kerak.");
        });
    }
}