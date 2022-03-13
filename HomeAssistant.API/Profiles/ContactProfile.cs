using AutoMapper;
using HomeAssistant.API.DTOs;
using HomeAssistant.API.Models;

namespace HomeAssistant.API.Profiles;

public class ContactProfile : Profile
{
    public ContactProfile()
    {
        CreateMap<AppUser, ContactDto>()
        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));
    }
}