using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShwubbApi.Data;

namespace ShwubbApi.Controllers
{

    [ApiController]
    [Route("profile/")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ProfileController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("get/profileposts")]
        public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? userId = null)
        {
            var query = _context.Posts.AsQueryable();

            if (userId != null)
            {
                query = query.Where(p => p.Userid == userId);
            }
            else if (userId == null)
            {
                return BadRequest(new { Message = "UserId query parameter is required." });
            }

                var totalPosts = await query.CountAsync();

            var posts = await query
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
                        ? $"{_config["AppSettings:ImageBaseUrl"].TrimEnd('/')}/{p.ImagePath.TrimStart('/')}"
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
