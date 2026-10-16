using CinemaHikes.Application.Dtos.Auth;
using FluentValidation;
namespace CinemaHikes.Application.Validators
{
    public class LoginRequestValidator: AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email не может быть пустым.")
                .MaximumLength(255)
                .WithMessage("Email слишком длинный");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль не может быть пустым");
        }
    }
}
