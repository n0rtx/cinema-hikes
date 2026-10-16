using CinemaHikes.Application.Dtos.Auth;
using FluentValidation;
namespace CinemaHikes.Application.Validators
{
    public class ChangePasswordRequestValidator: AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Старый пароль не может быть пустым");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Новый пароль пустой.")
                .MinimumLength(6)
                .WithMessage("Новый пароль слишком короткий")
                .MaximumLength(100)
                .WithMessage("Новый пароль слишком длинный")
                .NotEqual(x => x.OldPassword)
                .WithMessage("Новый пароль одинаковый с прошлым паролем");
        }
    }
}
