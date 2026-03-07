using FluentValidation;
using PhytoIntellect.Application.Contracts.Users;
using PhytoIntellect.Application.DTOs.AuthDTOs;
using PhytoIntellect.Application.DTOs.UserDTOs;

namespace YourProject.Contracts.Users
{

    //Registered in AddApplicationServiceDI 
    //The validator must use the model that is used in the controller => RegisterUserAuthDto
    public class UserValidator : AbstractValidator<UpdateUserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.UserName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^01[0-2,5]{1}[0-9]{8}$")
                .WithMessage("Phone number is not valid");

            RuleFor(x => x.Governorate)
                .NotEmpty();

            RuleFor(x => x.City)
                .NotEmpty();

            RuleFor(x => x.Street)
                .NotEmpty();

        }
    }
}