using System;
using System.Collections.Generic;
using System.Text;
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
                .WithMessage("Old password can not be empty");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Your new password is empty..")
                .MinimumLength(6)
                .WithMessage("New password is too short")
                .MaximumLength(100)
                .WithMessage("Password is too long")
                .NotEqual(x => x.OldPassword)
                .WithMessage("New password is new password not old...");
        }
    }
}
