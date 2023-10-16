using AutoMapper;
using MicrobUy_API.Dtos;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Models;
using MicrobUy_API.Services.AccountService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly IAccountService _accountService;

        public AccountController(UserManager<ApplicationUser> userManager, IMapper mapper, JwtHandler jwtHandler, IAccountService accountService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _accountService = accountService;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromHeader][Required] int tenant, [FromBody] UserRegistrationRequestDto userRegistration)
        {
             ApplicationUser userExist = null;
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();

            if (userRegistration == null || !ModelState.IsValid)
                return BadRequest();

            ApplicationUser user = _mapper.Map<ApplicationUser>(userRegistration);
            user.TenantInstanceId = tenant;

            if (tenant == 0)
                userExist = await _userManager.FindByNameAsync(user.Email);

            if (userExist != null)
            {
                listOfErrors.Add("El email ya está siendo utilizado");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });
            }

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

            UserModel bdUser = await _accountService.UserRegistration(userRegistration);

            if (bdUser == null)
            {
                listOfErrors.Add("Error no fue posible registrar el usuario");
                errors = listOfErrors.Select(x => x);

                return BadRequest(new UserRegistrationResponseDto { Errors = errors });
            }

            return StatusCode(201);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserAuthenticationRequestDto userForAuthentication)
        {
            ApplicationUser userExist = await _userManager.FindByNameAsync(userForAuthentication.Email);
            if (userExist == null || !await _userManager.CheckPasswordAsync(userExist, userForAuthentication.Password))
                return Unauthorized(new UserAuthenticationResponseDto { ErrorMessage = "Invalid Authentication" });
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = await _jwtHandler.GetClaims(userExist);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return Ok(new UserAuthenticationResponseDto { IsAuthSuccessful = true, Token = token });
        }

        [HttpGet("ObtenerUsuariosByInstance")]
        public async Task<IActionResult> ObtenerUsuariosByInstance()
        {
            IEnumerable<UserModel> usuarios = await _accountService.GetUsuarioByInstance();
            return Ok(usuarios);
        }

        [HttpGet("GetUsersByInstance")]
        public async Task<IActionResult> GetUsersByInstance()
        {
            IEnumerable<UserModel> usuarios = await _accountService.GetUsuarioByInstance();
            return Ok(usuarios);
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([Required] string userEmail)
        {
            UserModel user = await _accountService.GetUser(userEmail);
            return Ok(user);
        }
    }
}