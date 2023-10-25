using AutoMapper;
using Azure.Core;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;
        public async Task<PostModel> CreatePost(CreatePostDto post, string userEmail)
        {
            PostModel newPost = _mapper.Map<PostModel>(post);
            UserModel userExist = _context.User.Where(x => x.Email == userEmail && x.TenantInstanceId == 0).FirstOrDefault();

            if (userExist == null) return null;
            
            await _context.AddAsync(newPost);
            await _context.SaveChangesAsync();

            return newPost;
        }

   

        public Task<PostModel> CreatePostComment(PostModel post, CreatePostDto postComment, string userEmail)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userEmail)
        {
            throw new NotImplementedException();
        }
    }
}
