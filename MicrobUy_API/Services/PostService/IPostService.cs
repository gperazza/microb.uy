using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicrobUy_API.Services.PostService
{
    public interface IPostService
    {
<<<<<<< Updated upstream
       Task<PostModel> CreatePost(CreatePostDto post, string userEmail);
       Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userEmail);
=======
       Task<PostModel> CreatePost(CreatePostDto post, string userName);
       Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userName);
       Task<IEnumerable<PostDto>> GetPostByUser(string userName);
       Task<PostModel> LikeComment(int postId, string userName);
>>>>>>> Stashed changes
    }
}
