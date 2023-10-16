using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;


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

            await _context.AddAsync(newUser);
            _context.SaveChanges();

            return newUser;
        }

        public async Task<IEnumerable<UserModel>> GetUsuarioByInstance()
        {
            return _context.User.ToList();
        }

        public async Task<UserModel> GetUser(string userEmail)
        {
            return _context.User.Where(x => x.Email == userEmail).FirstOrDefault();
        }
    }
}