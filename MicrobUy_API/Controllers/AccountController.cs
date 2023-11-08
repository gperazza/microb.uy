using AutoMapper;
using Azure.Core;
using FluentValidation;
using MicrobUy_API.Dtos;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Models;
using MicrobUy_API.Services.AccountService;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly IAccountService _accountService;
        private readonly IInstanceService _tenantInstanceService;
        private IValidator<UserRegistrationRequestDto> _validator;

        public AccountController(UserManager<IdentityUser> userManager, IValidator<UserRegistrationRequestDto> validator, IMapper mapper, JwtHandler jwtHandler, IAccountService accountService, IInstanceService tenantInstanceService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _accountService = accountService;
            _tenantInstanceService = tenantInstanceService;
            _validator = validator;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequestDto userRegistration)
        {

            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();

            if (userRegistration == null || !ModelState.IsValid)
                return BadRequest();

            int tenantInstance = Convert.ToInt32(HttpContext.Request.Headers["tenant"]);


            IdentityUser user = _mapper.Map<IdentityUser>(userRegistration);

            if (tenantInstance != 0)
            {
                TenantInstanceModel instance = await _tenantInstanceService.GetInstance(Convert.ToInt32(HttpContext.Request.Headers["tenant"]));

                if (instance == null)
                {
                    listOfErrors.Add("La instancia no existe");
                    errors = listOfErrors.Select(x => x);

                    return BadRequest(new UserRegistrationResponseDto { Errors = errors });
                }

                user.UserName = user.UserName + "@" + instance.Dominio;
                userRegistration.Username = user.UserName;
            }
            else 
            { 
                user.UserName = user.UserName + "@" + "microbuy";
                userRegistration.Username = user.UserName;
            }

            var res = await _validator.ValidateAsync(userRegistration);
           
            if (res.IsValid)
            {
                var result = await _userManager.CreateAsync(user, userRegistration.Password);

                if (!result.Succeeded)
                {
                    errors = result.Errors.Select(e => e.Description);

                    return BadRequest(new UserRegistrationResponseDto { Errors = errors });
                }

                var roleResult = await _userManager.AddToRoleAsync(user, userRegistration.Role);

                if (!roleResult.Succeeded)
                {
                    errors = roleResult.Errors.Select(e => e.Description);

                    return BadRequest(new UserRegistrationResponseDto { Errors = errors });

                }

                userRegistration.Username = user.UserName;
                UserModel bdUser = await _accountService.UserRegistration(userRegistration);

                if (bdUser == null)
                {
                    listOfErrors.Add("Error no fue posible registrar el usuario");
                    errors = listOfErrors.Select(x => x);

                    return BadRequest(new UserRegistrationResponseDto { Errors = errors });
                }

                return Created("RegisterUser", bdUser);
            }

            return BadRequest(res.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserAuthenticationRequestDto userForAuthentication)
        {
            IdentityUser userExist = await _userManager.FindByNameAsync(userForAuthentication.Username);
            if (userExist == null || !await _userManager.CheckPasswordAsync(userExist, userForAuthentication.Password))
                return Unauthorized(new UserAuthenticationResponseDto { ErrorMessage = "Invalid Authentication" });
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = await _jwtHandler.GetClaims(userExist);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return Ok(new UserAuthenticationResponseDto { IsAuthSuccessful = true, Token = token });
        }

        [HttpPost("LoginSocialMedia")]
        public async Task<IActionResult> LoginSocialMedia()
        {
            string bearertoken = HttpContext.Request.Headers.Authorization;
            var handler = new JwtSecurityTokenHandler();
            var tokensplit = bearertoken.Split(' ');
            var jwtSecurityToken = handler.ReadJwtToken(tokensplit[1]);
            List<Claim> claimList = jwtSecurityToken.Claims.ToList();
            var claimEmail = claimList.Where(x => x.Type == "email").Select(y => y.Value).FirstOrDefault();

            IdentityUser userExist = await _userManager.FindByEmailAsync(claimEmail);
            if (userExist == null)
                return Ok(new UserAuthenticationResponseDto { ErrorMessage = "El usuario no existe" });
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = await _jwtHandler.GetClaims(userExist);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return Ok(new UserAuthenticationResponseDto { IsAuthSuccessful = true, Token = token });
        }


        [HttpGet("GetUsersByInstance")]
        public async Task<IActionResult> GetUsersByInstance()
        {
            IEnumerable<UserModel> usuarios = await _accountService.GetUsuarioByInstance();
            return Ok(usuarios);
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([Required] string userName)
        {
            UserModel user = await _accountService.GetUser(userName);
            return Ok(user);
        }

    }
}