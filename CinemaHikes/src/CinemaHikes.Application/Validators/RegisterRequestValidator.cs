using System;
using System.Collections.Generic;
using System.Text;
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
                .WithMessage("Enter your name")
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .MinimumLength(3);
            RuleFor(x =>x.Email).NotEmpty()
                .WithMessage("Email can not be empty.")
                .EmailAddress()
                .WithMessage("Incorrect email address.")
                .MaximumLength(255)
                .WithMessage("Email is too long..");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password can not be empty.")
                .MinimumLength(6)
                .WithMessage("Password is too short.") 
                .MaximumLength(100)
                .WithMessage("Password is too long.");
        }
    }
}
