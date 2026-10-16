using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Application.Dtos.Admin;
using FluentValidation;

namespace CinemaHikes.Application.Validators
{
    public sealed class ChangeReviewStatusValidator:AbstractValidator<ChangeReviewStatusDto>
    {
        public ChangeReviewStatusValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty()
                .WithMessage("Айди Review которое хотите поменять не может быть пустым");
            RuleFor(x => x.AdminId).NotNull().NotEmpty().WithMessage("Айди админа должно быть явно указано.");
        }
    }
}
