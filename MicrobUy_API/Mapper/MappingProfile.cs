using AutoMapper;
using MicrobUy_API.Dtos;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Models;
using Microsoft.AspNetCore.Identity;

namespace MicrobUy_API.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<UserRegistrationRequestDto, IdentityUser>().ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Username));
            CreateMap<UserAuthenticationRequestDto, IdentityUser>().ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Username));
            CreateMap<CreatePostDto, PostModel>();
            CreateMap<CreateTematicaRequestDto, TematicaModel>();
            CreateMap<ModifyUserRequestDto, UserModel>();
            CreateMap<CreatePostDto, CommentModel>();
            CreateMap<CityRequestDto, CityModel>();
            CreateMap<PostModel, PostDto>();
            CreateMap<UserModel, UserPostDto>();
            CreateMap<ModifyInstanceRequest, TenantInstanceModel>();
            CreateMap<UserRegistrationRequestDto, UserModel>();
            CreateMap<CreateInstanceRequestDto, TenantInstanceModel>();
            CreateMap<UserModel, FollowedUserDto>();
            CreateMap<UserModel, ResponseUserDto>();

        }
    }
}