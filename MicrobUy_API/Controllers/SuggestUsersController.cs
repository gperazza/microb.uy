using MicrobUy_API.Models;
using Microsoft.AspNetCore.Mvc;


namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuggestUsersController : ControllerBase
    {/*
        private readonly IGraphClient _client;

        public SuggestUsersController(IGraphClient client) {
            _client = client;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _client.Cypher.Match("(n: User)")
                                            .Return(n => n.As<UserModel>()).ResultsAsync;

            return Ok(users);
        }*/
    }
}
