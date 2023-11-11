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
        
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(CreateUserNeo4jDto createUserNeo4JDto)
        {
            await _neo4jUsersRepository.CreateUser(createUserNeo4JDto);
            return Ok();
        }

        [HttpPost("CreatePost")]
        public Task<int> CreatePost([FromBody] CrearPostNeo4jDto crearPostNeo4JDto)
        {
            return _neo4jUsersRepository.CreatePost(crearPostNeo4JDto);
        }

        [HttpPut("UpdateUser")]
        public Task<int> UpdateUser(CreateUserNeo4jDto createUserNeo4JDto)
        {
            return _neo4jUsersRepository.UpdateUser(createUserNeo4JDto);
        }

        [HttpPost("GiveLike")]
        public Task<int> GiveLike(int UserId, int tenantId, int postId)
        {
            return _neo4jUsersRepository.GiveLike(UserId, tenantId, postId);
        }

        [HttpGet("TopHashtagByTenant")]
        public Task<int> TopHashtagByTenant(int tenantId, int topCant)
        {
            return _neo4jUsersRepository.TopHashtagByTenant(tenantId, topCant);
        }

        [HttpGet("TopHashtagAllTenant")]
        public Task<int> TopHashtagAllTenant(int topCant)
        {
            return _neo4jUsersRepository.TopHashtagAllTenant(topCant);
        }
    }
}
