using MicrobUy_API.Data.Repositories;
using MicrobUy_API.Dtos;
using MicrobUy_API.Dtos.SuggestNeo4jDto;
using MicrobUy_API.Models;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.Intrinsics.X86;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuggestUsersController : ControllerBase
    {
        private readonly INeo4jUsersRepository _neo4jUsersRepository;
        private const int LIMITDATA = 15;

        public SuggestUsersController(INeo4jUsersRepository neo4jUsersRepository)
        {
            _neo4jUsersRepository = neo4jUsersRepository;
        }
        //Limite general para los top, asi se evita exceso de datos devueltos
        public int LimitTop(int topCant)
        {
            if (topCant == 0)
            {
                topCant = 5;
            }
            else if (topCant > LIMITDATA)
            {
                topCant = LIMITDATA;
            }
            return topCant;
        }
        /// <summary>
        /// Crea un usuario, en la base de datos Neo4j
        /// </summary>
        /// <param name="createUserNeo4JDto"></param>
        /// <returns></returns>
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserNeo4jDto createUserNeo4JDto)
        {
            await _neo4jUsersRepository.CreateUser(createUserNeo4JDto);
            return Ok();
        }

        /// <summary>
        /// Crea un post y lo relaciona con su creador, en la base de datos Neo4j
        /// </summary>
        /// <param name="crearPostNeo4JDto"></param>
        /// <returns></returns>
        [HttpPost("CreatePost")]
        public Task<int> CreatePost([FromBody] CrearPostNeo4jDto crearPostNeo4JDto)
        {
            return _neo4jUsersRepository.CreatePost(crearPostNeo4JDto);
        }

        /// <summary>
        /// Actualiza los datos del usuario, en la base de datos Neo4j
        /// </summary>
        /// <param name="createUserNeo4JDto"></param>
        /// <returns></returns>
        [HttpPut("UpdateUser")]
        public Task<int> UpdateUser([FromBody] CreateUserNeo4jDto createUserNeo4JDto)
        {
            return _neo4jUsersRepository.UpdateUser(createUserNeo4JDto);
        }

        /// <summary>
        /// Crea una relacion entre usuario y post llamada like
        /// </summary>
        /// <param name="giveLikeNeo4JDto"></param>
        /// <returns></returns>
        [HttpPost("GiveLike")]
        public Task<int> GiveLike([FromBody] GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            return _neo4jUsersRepository.GiveLike(giveLikeNeo4JDto);
        }

        /// <summary>
        /// Borra la relacion entre un usuario y un post llamada like
        /// </summary>
        /// <param name="giveLikeNeo4JDto"></param>
        /// <returns></returns>
        [HttpDelete("DeleteLike")]
        public Task<int> DeleteLike([FromBody] GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            return _neo4jUsersRepository.DeleteLike(giveLikeNeo4JDto);
        }

        /// <summary>
        /// Obtiene el top n Hasthag con mas relaciones en el tenan id pasado como parametro
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("TopHashtagByTenant")]
        public Task<List<HashtagNeo4jDto>> TopHashtagByTenant([Required]int tenantId, int topCant)
        {
            topCant = LimitTop(topCant);
            return _neo4jUsersRepository.TopHashtagByTenant(tenantId, topCant);
        }

        /// <summary>
        /// Obtiene el top n Hasthag con mas relaciones en todos los tenant
        /// </summary>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("TopHashtagAllTenant")]
        public Task<List<HashtagNeo4jDto>> TopHashtagAllTenant(int topCant)
        {
            topCant = LimitTop(topCant);
            return _neo4jUsersRepository.TopHashtagAllTenant(topCant);
        }

        /// <summary>
        /// Obtiene el top n Post con mas relaciones de tipo like en todos los tenant
        /// </summary>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("PostWhitMostLikeAllTenant")]
        public Task<List<PostWhitMostLikeNeo4jDto>> PostWhitMostLikeAllTenant(int topCant)
        {
            topCant = LimitTop(topCant);
            return _neo4jUsersRepository.PostWhitMostLikeAllTenant(topCant);
        }

        /// <summary>
        /// Obtiene el top n Post con mas relaciones de tipo like en el tenan id pasado como parametro
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("PostWhitMostLikeByTenant")]
        public Task<List<PostWhitMostLikeNeo4jDto>> PostWhitMostLikeByTenant([Required] int tenantId, int topCant)
        {
            topCant = LimitTop(topCant);
            return _neo4jUsersRepository.PostWhitMostLikeByTenant(tenantId, topCant);
        }

        /// <summary>
        /// Devuelve una lista de n usuarios con mas posibilidad de relacionarse con el userid pasado
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("SuggestUsersByTenant")]
        public Task<List<SuggestUserNeo4jDto>> SuggestUsersByTenant([Required] int tenantId, [Required] int userId, int topCant)
        {
            topCant = LimitTop(topCant);
            return _neo4jUsersRepository.SuggestUsersByTenant(tenantId, userId, topCant);
        }

        /// <summary>
        /// Ajusta los parametros que utilizara la funcion SuggestUsersByTenant, para recomendar usuarios
        /// </summary>
        /// <param name="senttingSuggestUsersNeo"></param>
        /// <returns></returns>
        [HttpPost("SenttingSuggestUsersAllTenant")]
        public async Task<IActionResult> SenttingSuggestUsersAllTenant([FromBody][Required] SenttingSuggestUsersNeo4jDto senttingSuggestUsersNeo)
        {
            if (senttingSuggestUsersNeo.LIVE > 100 || senttingSuggestUsersNeo.LIVE<0 ||
                senttingSuggestUsersNeo.LIKE > 100 || senttingSuggestUsersNeo.LIKE < 0 ||
                senttingSuggestUsersNeo.BORN > 100 || senttingSuggestUsersNeo.BORN < 0 ||
                senttingSuggestUsersNeo.HAVE > 100 || senttingSuggestUsersNeo.HAVE < 0 ||
                senttingSuggestUsersNeo.POSTED > 100 || senttingSuggestUsersNeo.POSTED < 0 ||
                senttingSuggestUsersNeo.WITH_HASHTAG > 100 || senttingSuggestUsersNeo.WITH_HASHTAG < 0)
            {
                return BadRequest("Los argumentos no estan dentro del rango natural 0 a 100 ");
            }
            else
            {
                await _neo4jUsersRepository.SenttingSuggestUsersAllTenant(senttingSuggestUsersNeo);
                return Ok();
            }
            
        }
    }
}
