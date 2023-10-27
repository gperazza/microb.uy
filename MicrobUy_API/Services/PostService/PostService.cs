using AutoMapper;
using Azure.Core;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using MicrobUy_API.Services.AccountService;

namespace MicrobUy_API.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;
        private readonly IAccountService _account;

        public PostService(TenantAplicationDbContext context, IMapper mapper, IAccountService account)
        {
            _mapper = mapper;
            _context = context;
            _account = account;
        }

        public async Task<PostModel> CreatePost(CreatePostDto post, string userEmail)
        {
            UserModel aux_user = await _account.GetUser(userEmail);
            PostModel aux_post = new PostModel();
            aux_post.UserOwner = aux_user;
            aux_post.isSanctioned = false;
            aux_post.Comments = null;
            aux_post.Likes = null;
            aux_post.Hashtag = post.Hashtag;
            aux_post.Attachment = post.Attachment;
            aux_post.Text = post.Text;
            aux_post.PostId = 0;
            Console.Write(aux_post);


            PostModel newPost = _mapper.Map<PostModel>(aux_post);
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
