namespace ShwubbApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ShwubbApi.Data;
    using ShwubbApi.Models;
    using ShwubbApi.Logic;
    using Microsoft.AspNetCore.Identity;
    using LoginRequest = Models.LoginRequest;
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Authorization;
    using System.ComponentModel.DataAnnotations;

    [ApiController]
    [Route("user/")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {

            {
                var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

                if (user == null)
                {
                    return Unauthorized(new { Message = "Invalid credentials" });
                }

                var passwordHasher = new PasswordHasher<ShwubbUser>();
                var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

                if (verificationResult != PasswordVerificationResult.Success)
                {
                    return Unauthorized(new { Message = "Invalid credentials" });
                }
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserID", user.Userid.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:ExpiryMinutes"]));

                var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    claims: claims,
                    expires: expiry,
                    signingCredentials: creds
                    );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    token = jwt,
                    username = user.Username,
                    role = user.Role
                });
            }
        }
        [Authorize]
        [HttpPost("authorize")]
        public IActionResult AuthorizeToken()
        {
            var username = HttpContext.User.Identity.Name;
            return Ok( new {username});
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("adminauth")]
        public IActionResult AuthorizeAdminToken()
        {
            
            return Ok();
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            {
                var emailValidator = new EmailAddressAttribute();

                if (string.IsNullOrWhiteSpace(request.Email) ||
                    !IsValidChecker.IsValidEmail(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password) || 
                    string.IsNullOrWhiteSpace(request.Username) ||
                    request.Password.Length < 8 ) 
                {
                    return BadRequest(new { message = "Invalid inputs" });
                }

                var existingUser = _context.Users
                                  .FirstOrDefault(u => u.Username == request.Username || u.Email == request.Email);
                
                if (existingUser != null)
                {
                    return Conflict(new { Message = "Username or Email already registered" });
                }

                PasswordHasher hasher = new PasswordHasher();
                string hashedPass = hasher.HashPassword(request.Password);

                var newUser = new ShwubbUser
                {
                    Username = request.Username,
                    Password = hashedPass,
                    Email = request.Email,
                    Role = "User"
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();

                var userCheck = _context.Users
                                  .FirstOrDefault(u => u.Username == request.Username);

                if (userCheck != null)
                {
                    return Ok(new { Message = "Registration Successful" });
                }
                else
                {
                    return Unauthorized(new { Message = "Registration Failed, try again" });
                }
            }
        }
    }
}
