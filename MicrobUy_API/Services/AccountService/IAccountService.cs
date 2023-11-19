using MicrobUy_API.Dtos;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.AccountService
{
    public interface IAccountService
    {
        Task<UserModel> UserRegistration(UserRegistrationRequestDto request);

        Task<IEnumerable<UserModel>> GetUsuarioByInstance();

        Task<UserModel> GetUser(string userName);

        Task<int> ModifyUser(ModifyUserRequestDto user);

        Task<IEnumerable<FollowedUserDto>> GetFollowedUsers(string userName);

        Task<int> FollowUser(string userName, string userNameToFollow);

        Task<IEnumerable<FollowedUserDto>> GetFollowers(string userName);
        
        Task<IEnumerable<FollowedUserDto>> GetMutedUsers(string userName);
        
        Task<IEnumerable<FollowedUserDto>> GetBlockedUsers(string userName);
        
        Task<int> MuteUser(string userName, string userNameToMute);
        
        Task<int> BlockUser(string userName, string userNameToBlock);

        Task<IEnumerable<PostDto>> GetUsersTimeLine(string userName);
    }
}