using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public record OrderRecipeRequest(int RecipeId, int Quantity);