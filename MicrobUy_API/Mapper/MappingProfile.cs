using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using AutoMapper;

namespace MicrobUy_API.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
    
            CreateMap<UserRegistrationRequestDto, UserModel>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
