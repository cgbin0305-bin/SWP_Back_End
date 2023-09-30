using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Helper;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Worker, WorkerDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.OrderHistories.Select(x => x.Review)))
            .ForMember(dest => dest.CountOrder, opt => opt.MapFrom(src => src.OrderHistories.Count()))
            .ForMember(dest => dest.AverageRate, opt => opt.MapFrom(src => src.OrderHistories != null && src.OrderHistories.Any() ?
            (int)src.OrderHistories.Average(x => x.Review != null ? x.Review.Rate : 0) :0))
            .ForMember(dest => dest.Chores, opt => opt.MapFrom(src => src.Workers_Chores.Select(x => x.Chore)));
        CreateMap<OrderHistory, OrderHistoryDto>()
            .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.Review));
        CreateMap<User, UserDto>();
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => string.Format("{0:yyyy-MM-dd}", src.Date)))
            .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.OrderHistory.GuestName));
        CreateMap<HouseHoldChores, HouseHoldChoresDto>();
    }
}