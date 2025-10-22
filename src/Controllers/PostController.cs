namespace ShwubbApi.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ShwubbApi.Data;
    using ShwubbApi.Logic;
    using ShwubbApi.Models;

    [ApiController]
    [Route("posts/")]
    public class PostsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PostsController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] PostRequest request)
        {
            if (request.Image != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg" };
                var extension = Path.GetExtension(request.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Unsupported file type.");
                }
            }

            using (var context = new AppDbContext())
            {
                int userId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

                string? imagePath = null;

                if (request.Image != null && request.Image.Length > 0)
                {
                    var uploadsDir = _config["AppSettings:ImageStoragePath"];
                    Directory.CreateDirectory(uploadsDir);

                    var fileName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(stream);
                    }

                    imagePath = "/" + fileName;
                }



                var newPost = new ShwubbPost 
                {
                Title = request.Title,
                Content = request.Content,
                ImagePath = imagePath,
                Userid = userId,
                CreatedAt = DateTime.Now
                };
                context.Posts.Add(newPost);
                await context.SaveChangesAsync();


                return Ok(new { Message= "Post uploaded successfully!"});
            }
        }

        [AllowAnonymous]
        [HttpGet("get")]
        public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            using (var context = new AppDbContext())
            {
                var totalPosts = await context.Posts.CountAsync();

                var posts = await context.Posts
                    .Include(p => p.Author)
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        p.Postid,
                        p.Title,
                        p.Content,
                        ImageUrl = p.ImagePath != null
                            ? $"{_config["AppSettings:ImageBaseUrl"].TrimEnd('/')}{p.ImagePath}"
                            : null,
                        Username = p.Author.Username,
                        p.Userid,
                        p.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalPosts = totalPosts,
                    Page = page,
                    PageSize = pageSize,
                    Posts = posts
                });
            }
        }

    }
}
