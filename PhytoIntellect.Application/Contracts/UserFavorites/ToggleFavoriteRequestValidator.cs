using FluentValidation;
using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.UserFavorites;

public class ToggleFavoriteRequestValidator : AbstractValidator<ToggleFavoriteRequest>
{
    public ToggleFavoriteRequestValidator()
    {
        RuleFor(x => x.TargetId)
            .GreaterThan(0)
            .WithMessage("Target ID must be greater than 0.");

        RuleFor(x => x.Type)
             .NotEmpty().WithMessage("Type field is required.")
             .IsEnumName(typeof(FavoriteType), caseSensitive: false)
             .WithMessage("Invalid Favorite Type. Allowed values are: 'Herb', 'Recipe', 'AiRecipe', 'Herbalist','AiChatRecipe'.");
    }
}