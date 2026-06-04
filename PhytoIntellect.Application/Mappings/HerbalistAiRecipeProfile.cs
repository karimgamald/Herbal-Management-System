using AutoMapper;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Mappings;

public class HerbalistAiRecipeProfile : Profile
{
    public HerbalistAiRecipeProfile()
    {
        // 1. من طلب الإضافة إلى الـ Entity الخاص بقاعدة البيانات
        CreateMap<AddAiRecipeToInventoryRequest, HerbalistAiRecipe>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

        // 2. من الـ Entity إلى الـ Response (تم دمج التكرار هنا بدقة)
        CreateMap<HerbalistAiRecipe, HerbalistAiRecipeResponse>()
            // 🚀 ربط معرف العشاب صراحة ليعمل مع الـ ProjectTo والـ Map اليدوي
            .ForMember(dest => dest.HerbalistId, opt => opt.MapFrom(src => src.HerbalistId))
            // دعم كلا المسميين إذا كنت تستخدمهما في الـ DTO لمنع الـ Null
            .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src => src.AiRecipe.RecommendedRecipeName));

        // 3. من الـ Entity إلى قائمة العشابين المتاحين للوصفة
        CreateMap<HerbalistAiRecipe, HerbalistWithAiRecipeResponse>()
            .ForMember(dest => dest.HerbalistId, opt => opt.MapFrom(src => src.HerbalistId)) // لضمان وصول المعرف هنا أيضاً
            .ForMember(dest => dest.HerbalistName, opt => opt.MapFrom(src => src.Herbalist.User!.FullName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                $"{src.Herbalist.User!.Governorate} - {src.Herbalist.User.City} - {src.Herbalist.User.Street}"));
    }
}