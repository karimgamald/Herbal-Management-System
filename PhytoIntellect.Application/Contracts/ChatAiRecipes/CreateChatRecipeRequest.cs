using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PhytoIntellect.Application.Contracts.ChatAiRecipes;

public class CreateChatRecipeRequest
{
    public string UserPrompt { get; set; } = string.Empty;
}