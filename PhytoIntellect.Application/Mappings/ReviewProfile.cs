using AutoMapper;
using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<ReviewRecipe, ReviewResponse>()
            .ForMember(dest => dest.HerbalistName,
                opt => opt.MapFrom(src => src.Herbalist != null && src.Herbalist.User != null
                    ? src.Herbalist.User.FullName
                    : "Unknown Herbalist"));

        CreateMap<SubmitReviewRequest, ReviewRecipe>();
    }
}