using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<UserModel>> GetUsuarioByInstance()
        {
            return _context.User.Where(x => !x.IsSanctioned)
                .Include(x => x.AdministratedInstances)
                .Include(x => x.Posts)
                .Include(x => x.Likes)
                .Include(x => x.City).ToList();
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
                if (check.City != user.City)
                {
                    check.City = newUser.City;
                    _context.Update(check);
                    _context.SaveChanges();
                }

                if (changeAuthEmail) { 
                
                    var currentIdentityUser = _IdentityContext.Users.Where(x => x.UserName == currentUser.FirstOrDefault().UserName && x.Email == oldEmail);
                    var resultIdentity = currentIdentityUser.ExecuteUpdate(setters => setters.SetProperty(b => b.Email, user.Email)
                                                .SetProperty(b => b.NormalizedEmail, user.Email.ToUpper()));
                    return resultIdentity;
                }
            
            }

            return result;
        }
    }
}