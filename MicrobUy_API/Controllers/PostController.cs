using AutoMapper;
using FluentValidation;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Paging;
using MicrobUy_API.Services.PostService;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private IValidator<CreatePostDto> _validator;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        // ------------------------------------------ FUNCIONALIDADES ABM POSTEOS ---------------------------------------- //

        /// <summary>
        /// Crea un posteo, para una instancia pasada por header
        /// </summary>
        /// <param name="post"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto post, string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
           //FluentValidation.Results.ValidationResult res = await _validator.ValidateAsync(post);
            
            //if(res.IsValid) {
                var result = await _postService.CreatePost(post, userName);
                if (result == null)
                {
                    listOfErrors.Add("Error al crear el post");
                    errors = listOfErrors.Select(x => x);
                    return BadRequest(new UserPostResponseDto { Errors = errors });
                }
                return Ok(result);
            //}
            //return BadRequest(res.Errors);
        }

        /// <summary>
        /// Comenta un posteo, para una instancia pasada por header
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="postComment"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("CreateComment")]
        public async Task<IActionResult> CommentAsync(int postId, [FromBody] CreatePostDto postComment, string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();

            var result = await _postService.CreatePostComment(postId, postComment, userName);
            if (result == null)
            {
                listOfErrors.Add("Error al crear el comentario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserPostResponseDto { Errors = errors });
            }
            return Ok(result);
        
        }

        /// <summary>
        /// Obtiene los post de un usuario, para una instancia pasada por header
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("GetPostByUser")]
        public async Task<IActionResult> GetPostByUser(String userName)
        {
            var result = await _postService.GetPostByUser(userName);
            return Ok(result);
        }

        /// <summary>
        /// Da like a un post, para una instancia pasada por header
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("AddLikeToPost")]
        public async Task<IActionResult> LikeAsync(int postId, string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();

            var result = await _postService.LikeComment(postId, userName);
            if (result == null)
            {
                listOfErrors.Add("Error al likear el posteo");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserPostResponseDto { Errors = errors });
            }
            return Ok(result);

        }

        /// <summary>
        /// Obtiene un post segun el id, para una instancia pasada por header
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("GetPostById")]
        public async Task<IActionResult> GetPostById(int postId)
        {
            var result = await _postService.GetPostById(postId);
            return Ok(result);
        }

        /// <summary>
        /// Borra un post segun el id, para una instancia pasada por header
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpPut("DeletePostById")]
        public async Task<IActionResult> DeletePostById(int postId)
        {
            var result = await _postService.DeletePostById(postId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene todos los post, para una instancia pasada por header
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpGet("GetAllPost")]
        public async Task<IActionResult> GetAllPost([FromQuery] PaginationParams @params)
        {
            var result = await _postService.GetAllPost();

            var paginationMetadata = new PaginationMetadata(result.Count(), @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = result.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                       .Take(@params.ItemsPerPage)
                                       .ToList();
      
            return Ok(items);
        }

        // ------------------------- FUNCIONALIDADES PARA REPORTAR Y MODERAR REPORTES ------------------------- //

        /// <summary>
        /// Reporta un post segun id, para una instancia pasada por header
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPut("ReportPost")]
        public async Task<IActionResult> ReportPostById(int postId, string userName)
        {
            var result = await _postService.ReportPostById(postId, userName);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene los post reportados, para una instancia pasada por header
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("GetReportedPost")]
        public async Task<IActionResult> GetReportedPosts()
        {
            var result = await _postService.GetReportedPosts();
            return Ok(result);
        }
        /// <summary>
        /// Da por valido un reporte con el id pasado, para una instancia pasada por header
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpPut("PunishPost")]
        public async Task<IActionResult> PunishPost(int postId)
        {
            var result = await _postService.PunishPost(postId);
            return Ok(result);
        }
        /// <summary>
        /// Da por rechazado un reporte con id, para una instancia pasada por header
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpPut("DismissReportedPost")]
        public async Task<IActionResult> DismissReportedPost(int postId)
        {
            var result = await _postService.DismissReportedPost(postId);
            return Ok(result);
        }

        // ------------------------------------- ENDPOINTS PARA ESTADISTICAS ------------------------------------- //
        /// <summary>
        /// cuenta la cantidad de post, para una instancia pasada por header
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("GetCountPost")]
        public async Task<IActionResult> GetCountPost()
        {
            var result = await _postService.GetCountPost();
            return Ok(result);
        }

    }
}
