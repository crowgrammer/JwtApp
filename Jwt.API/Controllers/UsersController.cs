using Jwt.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace Jwt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("get")]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await _context.Users.ToListAsync();
            if(users == null)
            {
                return NotFound("No Users Found");
            }
            return Ok(users);
        }
        [HttpPost("update")]
        public async Task<ActionResult<User>> update(int? id, UserPlain request)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            try
            {
                CreatePasswordHash(request.password, out byte[] hash, out byte[] salt);
                user.username = request.username;
                user.passwordHash = hash;
                user.passwordSalt = salt;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpPost("delete")]
        public async Task<ActionResult> Delete(int? id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            try
            {
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpPost("register"), AllowAnonymous]
        public async Task<ActionResult<User>> Register(UserPlain request) {

            User user = new();
            try
            {
            CreatePasswordHash(request.password, out byte[] hash, out byte[] salt );
            user.username = request.username;
            user.passwordHash = hash;
            user.passwordSalt = salt;
            user.role = "Customer";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            return Ok(user);
            }
            catch
            {
                return BadRequest();
            }

        }
        [HttpPost("CreateAdmin"), AllowAnonymous]
        public async Task<ActionResult<User>> CreateAdminUser(UserPlain request)
        {

            User user = new();
            try
            {
                CreatePasswordHash(request.password, out byte[] hash, out byte[] salt);
                user.username = request.username;
                user.passwordHash = hash;
                user.passwordSalt = salt;
                user.role = "Admin";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }

        }
        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<string>> Login(UserPlain request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.username == request.username);
            if(user == null)
            {
                return NotFound("User not found");
            }
            if(!VerifyPassword(request.password, user.passwordHash, user.passwordSalt)){
                return BadRequest("Wrong Password");
            }
            string token = CreateToken(user);
            return Ok(token);
        }







        //Helper Functions
        //TODO: Convert to Services
        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.role)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using( var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPassword(string password,  byte[] hash,  byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(hash);
            }
        }
    }
}
