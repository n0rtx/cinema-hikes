using CinemaHikes.Application.Dtos.Auth;
using FluentValidation;

namespace CinemaHikes.Application.Validators
{
    public class RegisterRequestValidator: AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Введите свое имя.")
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .MinimumLength(3);
            RuleFor(x =>x.Email).NotEmpty()
                .WithMessage("Email не может быть пустым.")
                .EmailAddress()
                .WithMessage("Неверный формат email.")
                .MaximumLength(255)
                .WithMessage("Email слишком длинный");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль не может быть пустым.")
                .MinimumLength(6)
                .WithMessage("Пароль слишком короткий.") 
                .MaximumLength(100)
                .WithMessage("Пароль слишком длинный.");
        }
    }
}
