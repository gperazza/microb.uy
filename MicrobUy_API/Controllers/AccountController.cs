using AutoMapper;
using FluentValidation;
using MicrobUy_API.Dtos;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Models;
using MicrobUy_API.Paging;
using MicrobUy_API.Services.AccountService;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

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

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userRegistration"></param>
        /// <returns></returns>
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

                //if (instance.Privacidad.)
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

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userForAuthentication"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpGet("GetUsersByInstance")]
        public async Task<IActionResult> GetUsersByInstance([FromQuery] PaginationParams @params)
        {
            IEnumerable<ResponseUserDto> usuarios = await _accountService.GetUsuarioByInstance();

            var paginationMetadata = new PaginationMetadata(usuarios.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = usuarios.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();
            return Ok(items);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([Required] string userName)
        {
            UserModel user = await _accountService.GetUser(userName);
            return Ok(user);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("ModifyUser")]
        public async Task<IActionResult> ModifyUser([FromBody] ModifyUserRequestDto user)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            var result = await _accountService.ModifyUser(user);

            if (result != 1)
            {
                listOfErrors.Add("No fue posible modificar el usario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });

            }
            return Ok(result);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userNameToFollow"></param>
        /// <returns></returns>
        [HttpPut("FollowUser")]
        public async Task<IActionResult> FollowUser(string userName, string userNameToFollow)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _accountService.FollowUser(userName, userNameToFollow);

            if (result != 2)
            {
                listOfErrors.Add("No fue posible seguir al usuario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });

            }

            return Ok(result);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userNameToBlock"></param>
        /// <returns></returns>
        [HttpPut("BlockUser")]
        public async Task<IActionResult> BlockUser(string userName, string userNameToBlock)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _accountService.BlockUser(userName, userNameToBlock);

            if (result != 2)
            {
                listOfErrors.Add("No fue posible bloquear al usuario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });

            }

            return Ok(result);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userNameToMute"></param>
        /// <returns></returns>
        [HttpPut("MuteUser")]
        public async Task<IActionResult> MutekUser(string userName, string userNameToMute)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _accountService.MuteUser(userName, userNameToMute);

            if (result != 1)
            {
                listOfErrors.Add("No fue posible mutear al usuario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });

            }

            return Ok(result);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="params"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("GetFollowedUsers")]
        public async Task<IActionResult> GetFollowedUsers([FromQuery] PaginationParams @params, string userName)
        {
            IEnumerable<FollowedUserDto> usuarios = await _accountService.GetFollowedUsers(userName);

            var paginationMetadata = new PaginationMetadata(usuarios.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = usuarios.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();

            return Ok(items);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="params"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("GetFollowers")]
        public async Task<IActionResult> GetFollowers([FromQuery] PaginationParams @params, string userName)
        {
            IEnumerable<FollowedUserDto> usuarios = await _accountService.GetFollowers(userName);

            var paginationMetadata = new PaginationMetadata(usuarios.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = usuarios.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();

            return Ok(items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="params"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("GetBlockedUsers")]
        public async Task<IActionResult> GetBlockedUsers([FromQuery] PaginationParams @params, string userName)
        {
            IEnumerable<FollowedUserDto> usuarios = await _accountService.GetBlockedUsers(userName);

            var paginationMetadata = new PaginationMetadata(usuarios.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = usuarios.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();

            return Ok(items);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="params"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("GetMutedUsers")]
        public async Task<IActionResult> GetMutedUsers([FromQuery] PaginationParams @params, string userName)
        {
            IEnumerable<FollowedUserDto> usuarios = await _accountService.GetMutedUsers(userName);

            var paginationMetadata = new PaginationMetadata(usuarios.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = usuarios.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();

            return Ok(items);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="params"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("GetUserTimeline")]
        public async Task<IActionResult> GetUserTimeline([FromQuery] PaginationParams @params, string userName)
        {
            IEnumerable<PostDto> timeline = await _accountService.GetUsersTimeLine(userName);

            var paginationMetadata = new PaginationMetadata(timeline.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = timeline.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();
            
            return Ok(items.OrderByDescending(x => x.Created));
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPut("SancionateUser")]
        public async Task<IActionResult> SancionateUser(string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _accountService.SancionateUser(userName);

            if (result != 1)
            {
                listOfErrors.Add("No fue posible sancionar al usuario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });

            }

            return Ok(result);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpGet("GetSanctionedUsers")]
        public async Task<IActionResult> GetSanctionedUsers([FromQuery] PaginationParams @params)
        {
            IEnumerable<FollowedUserDto> sancionatedUsers = await _accountService.GetSancionatedUsers();

            var paginationMetadata = new PaginationMetadata(sancionatedUsers.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = sancionatedUsers.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();
            return Ok(items);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPut("ActiveUser")]
        public async Task<IActionResult> ActiveUser(string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _accountService.ActiveUser(userName);

            if (result != 1)
            {
                listOfErrors.Add("No fue posible activar al usuario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });

            }

            return Ok(result);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpGet("GetInactiveUsers")]
        public async Task<IActionResult> GetInactiveUsers([FromQuery] PaginationParams @params)
        {
            IEnumerable<FollowedUserDto> inactivedUsers = await _accountService.GetInactiveUsers();

            var paginationMetadata = new PaginationMetadata(inactivedUsers.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = inactivedUsers.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();
            return Ok(items);
        }
    }
}