using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Herb
{
    public int HerbId { get; set; }
    public string HerbName { get; set; } = string.Empty;
    public string? ScientificName { get; set; }
    public string? Description { get; set; }
    public string? Benefits { get; set; }
    public string? Dosage { get; set; }
    public string? Warnings { get; set; }
    public string? ImageURL { get; set; }

    // ==========================================
    // التعديل الجديد عشان نحل مشكلة إضافة العطار
    // ==========================================

    // 1. هل العشبة دي السيستم وافق عليها وبقت عامة ولا لسه قيد المراجعة؟
    public bool IsApproved { get; set; } = false;

    // 2. مين العطار اللي اقترح العشبة دي؟ (عشان نبعتله إشعار لما يتوافق عليها)
    // لاحظ إنها نلابل عشان الأعشاب اللي السيستم ضايفها بنفسه ملهاش عطار
    public int? AddedByHerbalistId { get; set; }
    public Herbalist? AddedByHerbalist { get; set; }

    // ==========================================

    // العلاقات القديمة زي ما هي
    public ICollection<RecipeHerb> RecipeHerbs { get; set; } = [];
    public ICollection<HerbalistHerb> HerbalistHerbs { get; set; } = [];
}
