using MicrobUy_API.Data.Repositories;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuggestUsersController : ControllerBase
    {
        private readonly INeo4jUsersRepository _neo4jUsersRepository;

        public SuggestUsersController(INeo4jUsersRepository neo4jUsersRepository)
        {
            _neo4jUsersRepository = neo4jUsersRepository;
        }
        
        [HttpGet("CreateUser")]
        public async Task<ActionResult> CreateUser(int UserId, int tenantId, string username, string occupation, string city)
        {
            await _neo4jUsersRepository.CreateUser(UserId, tenantId, username, occupation, city);
            return Ok();
        }

        [HttpGet("CreatePost")]
        public Task<int> CreatePost(int UserId, int tenantId, int postId, string postCreated/*,List<String> hashtag*/)
        {
            return _neo4jUsersRepository.CreatePost(UserId, tenantId, postId, postCreated/*, hashtag*/);
        }

        [HttpGet("UpdateUser")]
        public Task<int> UpdateUser(int UserId, int tenantId, string username, string occupation, string city)
        {
            return _neo4jUsersRepository.UpdateUser(UserId, tenantId, username, occupation, city);
        }
    }
}
