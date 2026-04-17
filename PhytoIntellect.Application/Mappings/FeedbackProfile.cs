using AutoMapper;
using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Mappings;

public class FeedbackProfile : Profile
{
    public FeedbackProfile()
    {
        CreateMap<Feedback, FeedbackResponse>()
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null
                    ? src.Patient.User.FullName
                    : "Unknown Patient"));
    }
}