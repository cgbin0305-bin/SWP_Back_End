using API.DTOs;
using API.Entities;
using AutoMapper;
using DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Helper;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Worker, WorkerDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.OrderHistories.Select(x => x.Review).Where(x => x != null))) // Set to an empty list if null // filter out any null reviews
            .ForMember(dest => dest.CountOrder, opt => opt.MapFrom(src => src.OrderHistories != null && src.OrderHistories.Any() ? src.OrderHistories.Count() : 0))
            .ForMember(dest => dest.AverageRate, opt => opt.MapFrom(src => src.OrderHistories != null && src.OrderHistories.Any(x => x.Review != null) ?
            (int)src.OrderHistories.Where(x => x.Review != null).Average(x => x.Review.Rate) : 0))
            .ForMember(dest => dest.Chores, opt => opt.MapFrom(src => src.Workers_Chores.Select(x => x.Chore)));
        CreateMap<WorkerDto, WorkerPage>();
        CreateMap<OrderHistory, OrderHistoryDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => string.Format("{0:yyyy-MM-dd}", src.Date)))
            .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.Worker.Id))
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Review != null ? src.Review.Rate : 0))
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker.User.Name));
        CreateMap<User, UserDto>();
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => string.Format("{0:yyyy-MM-dd}", src.Date)))
            .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.OrderHistory.GuestName));
        CreateMap<HouseHoldChores, HouseHoldChoresDto>();
        CreateMap<HireWorkerInfoDto, OrderHistory>();
        CreateMap<WorkerUpdateByAdminDto, Worker>()
        .BeforeMap((src, dest) =>
        {
            dest.User.Name = src.Name;
            dest.User.Address = src.Address;
        })
        .ForMember(dest => dest.Workers_Chores, opt => opt.MapFrom(src => src.Chores.Select(x => new Workers_Chores { WorkerId = src.Id, ChoreId = x })))
        .ForMember(dest => dest.Version, opt => opt.Ignore());

        CreateMap<RegisterDto, User>()
        .BeforeMap((src, dest) =>
        {
            var hmac = src.Password.GetPasswordHash();
            dest.PasswordHash = hmac.Item1;
            dest.PasswordSalt = hmac.Item2;
        });

        // CreateMap<AccountUpdateDto, User>()
        // .BeforeMap((src, dest) =>
        // {
        //     var hmac = src.Password.GetPasswordHash();
        //     dest.PasswordHash = hmac.Item1;
        //     dest.PasswordSalt = hmac.Item2;
        // })
        // .ForMember(dest => dest.Version, opt => opt.Ignore());

        CreateMap<OrderHistory, OrderHistoryOfWorkerDto>()
            .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.Review))
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Review != null ? src.Review.Rate : 0));
        CreateMap<OrderHistory, OrderHistoryOfUserDto>()
            .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.Review))
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker.User.Name));
    }
}