using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Herb : LocalizedEntity
{
    public int HerbId { get; set; }
    public string HerbName { get; set; } = string.Empty;
    public string? ScientificName { get; set; }
    public string? Description { get; set; }
    public string? Benefits { get; set; }
    public string? Dosage { get; set; }
    public string? Warnings { get; set; }
    public string? ImageURL { get; set; }

    public bool IsApproved { get; set; } = false;

    public int? AddedByHerbalistId { get; set; }
    public Herbalist? AddedByHerbalist { get; set; }

    public ICollection<RecipeHerb> RecipeHerbs { get; set; } = [];
    public ICollection<HerbalistHerb> HerbalistHerbs { get; set; } = [];
}
