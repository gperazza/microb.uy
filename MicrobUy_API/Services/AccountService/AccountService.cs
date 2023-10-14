using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        public async Task<IEnumerable<UserModel>> GetUsuarioByTenant(int tenant)
        {
            IEnumerable<UserModel> usuarios = null;
            TenantInstanceModel tenantInstance = _context.TenantInstances.Where(x=>x.TenantInstanceId==tenant && x.Activo!=false).FirstOrDefault();
            if (tenant != 0 || tenantInstance != null ) {
                usuarios = _context.User.ToList().Where(x => x.TenantInstanceId == tenant);
            }
            return usuarios;
        }

    }
}