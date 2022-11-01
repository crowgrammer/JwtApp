using Jwt.API.Models;
using Jwt.API.Services;
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
        private readonly IPasswordService _passwordService;
        public UsersController(AppDbContext context,
           IConfiguration configuration,
           IPasswordService passwordService)
        {
            _context = context;
            _configuration = configuration;
            _passwordService = passwordService;
        }
        [HttpGet("getAllUsers")]
        public async Task<ActionResult<List<User>>> GetAllusers()
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
                _passwordService.CreatePasswordHash(request.password, out byte[] hash, out byte[] salt);
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
            _passwordService.CreatePasswordHash(request.password, out byte[] hash, out byte[] salt );
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
        [HttpPost("CreateAdmin")]
        public async Task<ActionResult<User>> CreateAdminUser(UserPlain request)
        {

            User user = new();
            try
            {
                _passwordService.CreatePasswordHash(request.password, out byte[] hash, out byte[] salt);
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
            if(!_passwordService.VerifyPassword(request.password, user.passwordHash, user.passwordSalt)){
                return BadRequest("Wrong Password");
            }
            string token = _passwordService.CreateToken(user, _configuration.GetSection("AppSettings:Token").Value);
            return Ok(token);
        }
    }
}
