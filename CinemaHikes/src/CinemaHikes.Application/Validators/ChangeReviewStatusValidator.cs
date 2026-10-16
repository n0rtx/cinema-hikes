using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Domain.Enums;
using FluentValidation;

namespace CinemaHikes.Application.Validators
{
    public sealed class ChangeReviewStatusValidator:AbstractValidator<ChangeReviewStatusDto>
    {
        public ChangeReviewStatusValidator()
        {
            RuleFor(x => x.newStatus).IsInEnum()
                .NotEmpty()
                .WithMessage("Новый статус не может быть пустым");
        }
    }
}
