using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicrobUy_API.Services.PostService
{
    public interface IPostService
    {
       Task<PostModel> CreatePost(CreatePostDto post, string userName);
       Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userName);
       Task<bool> DeletePostById(int postId);
       Task<bool> DismissReportedPost(int postId);
       Task<CountPostDto> GetCountPost();
       Task<PostDto> GetPostById(int postId);
       Task<IEnumerable<PostDto>> GetPostByUser(string userName);
       Task<IEnumerable<PostDto>>  GetReportedPosts();
       Task<IEnumerable<PostDto>> GetAllPost();
       Task<PostModel> LikeComment(int postId, string userName);
       Task<bool> PunishPost(int postId);
       Task<bool> ReportPostById(int postId, string userName);
    }
}
