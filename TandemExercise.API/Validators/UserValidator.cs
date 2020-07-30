using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TandemExercise.Business.Entities;

namespace TandemExercise.API
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.emailAddress).NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("A valid email is required");
            RuleFor(x => x.firstName).NotEmpty();
            RuleFor(x => x.lastName).NotEmpty();
        }
    }
}
