using AutoMapper;
using Firebase.Auth;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MicrobUy_API.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;
        private readonly IdentityProviderDbContext _IdentityContext;

        public AccountService(IdentityProviderDbContext IdentityContext, TenantAplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _IdentityContext = IdentityContext;
        }

        public async Task<UserModel> UserRegistration(UserRegistrationRequestDto request)
        {
            UserModel newUser = _mapper.Map<UserModel>(request);
            newUser.CreationDate = DateTime.Now;    

            await _context.AddAsync(newUser);
            _context.SaveChanges();

            return newUser;
        }

        public async Task<IEnumerable<ResponseUserDto>> GetUsuarioByInstance()
        {
            List<ResponseUserDto> usersDto = new List<ResponseUserDto>();
            var users = _context.User.Where(x => !x.IsSanctioned)
                .Include(x => x.City).ToList();

            if (users.Any())
                usersDto = _mapper.Map<List<UserModel>, List<ResponseUserDto>>(users);

            return usersDto;

        }

        public async Task<UserModel> GetUser(string userName)
        {
            return _context.User
                .Include(x => x.AdministratedInstances)
                .Include(x => x.Posts)
                .Include(x => x.Likes)
                .Include(x => x.City)
              .FirstOrDefault(x => x.UserName == userName && !x.IsSanctioned);
        }

        public async Task<int> ModifyUser(ModifyUserRequestDto user)
        {
            UserModel newUser = _mapper.Map<UserModel>(user);
            bool changeAuthEmail = false;
            string oldEmail = "";

            var currentUser = _context.User.Where(b => b.TenantInstanceId == _context._tenant && b.UserId == user.UserId);


            if (currentUser.FirstOrDefault().Email != user.Email) { changeAuthEmail = true;  oldEmail = currentUser.FirstOrDefault().Email;}
                
                
                var result = currentUser.ExecuteUpdate(setters => setters.SetProperty(b => b.FirstName, user.FirstName)
                                                .SetProperty(b => b.LastName, user.LastName)
                                                .SetProperty(b => b.Email, user.Email)
                                                .SetProperty(b => b.Biography, user.Biography)
                                                .SetProperty(b => b.ProfileImage, user.ProfileImage)
                                                .SetProperty(b => b.Birthday, user.Birthday)
                                                .SetProperty(b => b.Occupation, user.Occupation));
            if (result == 1)
            {
                var check = _context.User.Include(b => b.City).FirstOrDefault(b => b.TenantInstanceId == _context._tenant && b.UserId == user.UserId);
                if (check.City.Id != user.City.Id)
                {
                    check.City = newUser.City;
                    _context.Update(check);
                    _context.SaveChanges();
                }

                if (changeAuthEmail) { 
                
                    var currentIdentityUser = _IdentityContext.Users.Where(x => x.UserName == currentUser.FirstOrDefault().UserName && x.Email == oldEmail);
                    var resultIdentity = currentIdentityUser.ExecuteUpdate(setters => setters.SetProperty(b => b.Email, user.Email)
                                                .SetProperty(b => b.NormalizedEmail, user.Email.ToUpper()));
                    
                    result = resultIdentity;
                }


                var currentUserToChangeRole = _IdentityContext.Users.Where(x => x.UserName == currentUser.FirstOrDefault().UserName);
                
                var userRoles = _IdentityContext.UserRoles.Where(x => x.UserId == currentUserToChangeRole.FirstOrDefault().Id);
                var roles = _IdentityContext.Roles;

                if (userRoles.FirstOrDefault().RoleId != roles.FirstOrDefault(x => x.Name == user.Role).Id)
                {
                    int resultRoles = userRoles.ExecuteUpdate(setters => setters.SetProperty(b => b.RoleId, roles.FirstOrDefault(x => x.Name == user.Role).Id));

                    result = resultRoles;
                }
            }

            return result;
        }

        public async Task<int> FollowUser(string userName, string userNameToFollow)
        {
            UserModel user = _context.User.FirstOrDefault(x => x.UserName == userName);
            UserModel userToFollow = _context.User.FirstOrDefault(x => x.UserName == userNameToFollow);

            if (user == null || userToFollow == null)
                return 0;
           
            user.Following.Add(userToFollow);
            userToFollow.Followers.Add(user);

            return _context.SaveChanges();

        }

        public async Task<IEnumerable<FollowedUserDto>> GetFollowedUsers(string userName)
        {
            List<FollowedUserDto> following = new List<FollowedUserDto>();
            UserModel user = _context.User.Include(x => x.Following).FirstOrDefault(x => x.UserName == userName);
            
            if (user != null)
                following = _mapper.Map<List<UserModel>, List<FollowedUserDto>>(user.Following.ToList());

            return following;

        }

        public async Task<IEnumerable<FollowedUserDto>> GetFollowers(string userName)
        {
           List<FollowedUserDto> followers = new List<FollowedUserDto>();
           UserModel user = _context.User.Include(x => x.Followers).FirstOrDefault(x => x.UserName == userName);
            
            if(user != null)
                followers = _mapper.Map<List<UserModel>, List<FollowedUserDto>>(user.Followers.ToList());

            return followers;

        }

        public async Task<int> BlockUser(string userName, string userNameToBlock)
        {
            UserModel user = _context.User.Include(x => x.Following).Include(x => x.Followers).FirstOrDefault(x => x.UserName == userName);
            UserModel userToBlock = _context.User.FirstOrDefault(x => x.UserName == userNameToBlock);

            if (user == null || userToBlock == null)
                return 0;

            if (user.Followers.Contains(userToBlock)) { 
                
                user.Followers.Remove(userToBlock); 
            }

            if (user.Following.Contains(userToBlock))
            {

                user.Following.Remove(userToBlock);
            }

            user.BlockUsers.Add(userToBlock);
          
            return _context.SaveChanges();

        }

        public async Task<int> MuteUser(string userName, string userNameToMute)
        {
            UserModel user = _context.User.FirstOrDefault(x => x.UserName == userName);
            UserModel userToMute = _context.User.FirstOrDefault(x => x.UserName == userNameToMute);

            if (user == null || userToMute == null)
                return 0;

            user.MuteUsers.Add(userToMute);

            return _context.SaveChanges();

        }

        public async Task<IEnumerable<FollowedUserDto>> GetBlockedUsers(string userName)
        {
            List<FollowedUserDto> blockedUsers = new List<FollowedUserDto>();
            UserModel user = _context.User.Include(x => x.BlockUsers).FirstOrDefault(x => x.UserName == userName);

            if (user != null)
                blockedUsers = _mapper.Map<List<UserModel>, List<FollowedUserDto>>(user.BlockUsers.ToList());

            return blockedUsers;

        }

        public async Task<IEnumerable<FollowedUserDto>> GetMutedUsers(string userName)
        {
            List<FollowedUserDto> mutedUsers = new List<FollowedUserDto>();
            UserModel user = _context.User.Include(x => x.MuteUsers).FirstOrDefault(x => x.UserName == userName);

            if (user != null)
                mutedUsers = _mapper.Map<List<UserModel>, List<FollowedUserDto>>(user.MuteUsers.ToList());

            return mutedUsers;

        }

        public async Task<IEnumerable<PostDto>> GetUsersTimeLine(string userName)
        {
            List<PostDto> userTimeLine = new List<PostDto>();
           
            List<PostModel> userPosts = _context.Post.Include(y => y.UserOwner).Include(y => y.Likes).Include(y => y.Comments).Include(y => y.Hashtag)
                .Where(y => y.UserOwner.UserName == userName && !y.isSanctioned && !(y is CommentModel)).ToList();

            List<int> postsIds = _context.User.Include(x => x.MuteUsers).Include(x => x.Following)
                .Where(x => x.UserName == userName).SelectMany(x => x.Following.Where(y => !x.MuteUsers.Contains(y))) 
                .SelectMany(x => x.Posts).Where(x => !x.isSanctioned && !(x is CommentModel)).Select(x => x.PostId).ToList();

            List<PostModel> userFollowingPosts = _context.Post.Include(y => y.UserOwner).Include(y => y.Likes).Include(y => y.Comments).Include(y => y.Hashtag)
                .Where(y => postsIds.Contains(y.PostId)).ToList();

            if (userFollowingPosts.Any())
            {
                if (userPosts.Any())
                {
                    userFollowingPosts.AddRange(userPosts);
                }

                userTimeLine = _mapper.Map<List<PostModel>, List<PostDto>>(userFollowingPosts);

            }
            else
            {
                if (userPosts.Any())
                {

                    userTimeLine = _mapper.Map<List<PostModel>, List<PostDto>>(userPosts);

                }
            }

            return userTimeLine;
        }

        public async Task<int> SancionateUser(string userName)
        {
            UserModel user = _context.User.FirstOrDefault(x => x.UserName == userName);

            if (user == null)
                return 0;

            user.IsSanctioned = true;

            return _context.SaveChanges();

        }

        public async Task<IEnumerable<FollowedUserDto>> GetSancionatedUsers()
        {
            List<FollowedUserDto> sacionatedUsers = new List<FollowedUserDto>();
            List<UserModel> user = _context.User.Where(x => x.IsSanctioned).ToList();

            if (user != null)
                sacionatedUsers = _mapper.Map<List<UserModel>, List<FollowedUserDto>>(user);

            return sacionatedUsers;

        }

        public async Task<int> ActiveUser(string userName)
        {
            UserModel user = _context.User.FirstOrDefault(x => x.UserName == userName);

            if (user == null)
                return 0;

            user.Active = true;

            return _context.SaveChanges();

        }

        public async Task<IEnumerable<FollowedUserDto>> GetInactiveUsers()
        {
            List<FollowedUserDto> inactiveUsers = new List<FollowedUserDto>();
            List<UserModel> users = _context.User.Where(x => !x.Active).ToList();

            if (users != null)
                inactiveUsers = _mapper.Map<List<UserModel>, List<FollowedUserDto>>(users);

            return inactiveUsers;

        }
    }
}