using Jwt.API.Models;
using Jwt.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jwt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SeedData : ControllerBase
    {
        private static AppDbContext _context;
        private static IPasswordService _passwordService;

        public SeedData(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }
        [HttpPost("Seed Database")]
        public ActionResult Seed()
        {
            if (!_context.Users.Any())
            {
                _passwordService.CreatePasswordHash("admin", out byte[] hash, out byte[] salt);
                User admin = new()
                {
                    username = "admin",
                    passwordHash = hash,
                    passwordSalt = salt,
                    role = "Admin"
                };
                _context.Users.Add(admin);
                _context.SaveChanges();

            }
            if (_context.orderStatuse.Any())
            {
                _context.orderStatuse.AddRange(
                new OrderStatus()
                {
                    Status = "Created"
                },
                new OrderStatus()
                {
                    Status = "Ordered"
                },
                new OrderStatus()
                {
                    Status = "Shipped"
                });
            }
            return Ok();

        }
    }
}
