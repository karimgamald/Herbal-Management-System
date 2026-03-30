using AutoMapper;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Core.Entities;

public class SubOrderProfile : Profile
{
    public SubOrderProfile()
    {
        CreateMap<SubOrder, SubOrderSummaryResponse>();
        // لو حبيت تضيف Maps تانية تخص العطار بس حطها هنا

        CreateMap<SubOrder, SubOrderSummaryResponse>();
        //.ForMember(dest => dest.HerbalistName, opt => opt.MapFrom(src => src.Herbalist!.User!.FullName));
        // 👆 عدل .User.FullName حسب طريقة تخزينك لاسم العطار في الداتابيز
    }
}