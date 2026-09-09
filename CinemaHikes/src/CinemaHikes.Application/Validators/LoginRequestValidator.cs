using System;
using System.Collections.Generic;
using System.Text;
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
                .WithMessage("Email or username can not be empty you idiot.")
                .MaximumLength(255)
                .WithMessage("Your email is too long for us....");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password can not be empty??");
        }
    }
}
