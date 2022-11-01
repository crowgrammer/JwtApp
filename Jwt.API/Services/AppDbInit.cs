using Jwt.API.Models;
using Microsoft.Extensions.Configuration;

namespace Jwt.API.Services
{
    public class AppDbInit
    {
        private static AppDbContext _context;
        private static IPasswordService _passwordService;

        public AppDbInit(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }
       
        public static void Seed()
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
            
        }
    }
}
