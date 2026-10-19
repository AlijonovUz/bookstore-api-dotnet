using FluentValidation;
using FirstBackendApp.Contracts.Auth.Requests;

namespace FirstBackendApp.Contracts.Auth.Validators;

// Tizimga kirish so'rovini tekshiruvchi validator
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        // Username tekshiruvi
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Foydalanuvchi nomi kiritilishi shart.");
        
        // Parol tekshiruvi
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol kiritilishi shart.");
    }
}