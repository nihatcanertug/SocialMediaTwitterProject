using FluentValidation;
using SocialMediaTwitterProject.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMediaTwitterProject.Application.Validations
{
    public class LoginValidation : AbstractValidator<LoginDTO>
    {
        public LoginValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Enter a username");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Enter a password");
        }
    }
}
