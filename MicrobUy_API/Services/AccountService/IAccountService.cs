﻿using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.AccountService
{
    public interface IAccountService
    {
        Task<UserModel> UserRegistration(UserRegistrationRequestDto request);
    }
}