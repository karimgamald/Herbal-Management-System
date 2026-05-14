using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.ChatAiRecipes;

public class CreateChatRecipeRequestValidator : AbstractValidator<CreateChatRecipeRequest>
{
    public CreateChatRecipeRequestValidator()
    {
        RuleFor(x => x.UserPrompt)
        .NotEmpty().WithMessage("Please describe your symptoms. This field cannot be empty.")
        .MaximumLength(300).WithMessage("Your description is too long! Please keep it under 300 characters.")
        .Matches(@"^[\p{L}\p{N}\s\.,!?'-]+$")
        .WithMessage("Please use only standard text. Special characters and symbols are not allowed.");
    }
}