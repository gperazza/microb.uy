using AutoMapper;
using Azure.Core;
using MicrobUy_API.Data;
using MicrobUy_API.Models;
<<<<<<< Updated upstream
using MicrobUy_API.Services.AccountService;
=======
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Dtos;
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
        public Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userEmail)
        {
            throw new NotImplementedException();
        }
=======
        public async Task<IEnumerable<PostDto>> GetPostByUser(string userName)
        {
            var aux_post = _context.Post.Where(x => x.UserOwner.UserName == userName && !(x is CommentModel)).Include(x => x.Comments).Include(x => x.UserOwner)
               .Include(x => x.Likes).Include(x => x.Hashtag).Include(X=> X.Likes).ToList();

            var postDto = _mapper.Map<List<PostModel>, List<PostDto>>(aux_post);
            return postDto;
        }

        public async Task<PostModel> LikeComment(int postId, string userName)
        {
            PostModel aux_post = _context.Post.FirstOrDefault(x => x.PostId == postId);
            UserModel aux_user = _context.User.Where(x => x.UserName == userName).FirstOrDefault();

            if (aux_user == null) return null;
            if (aux_post == null) return null;

            aux_post.Likes.Add(aux_user);
            _context.SaveChanges();

            return aux_post;
        }

 

    }

>>>>>>> Stashed changes
    }
}
