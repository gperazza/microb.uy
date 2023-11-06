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

        public AccountService(TenantAplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
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
            return _context.User.ToList();
        }

        public async Task<UserModel> GetUser(string userName)
        {
            return _context.User
                .Include(x => x.AdministratedInstances)
                .Include(x => x.Posts)
                .Include(x => x.Likes)
              .FirstOrDefault(x => x.UserName == userName);
        }

    }
}