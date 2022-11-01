using Jwt.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public OrdersController(AppDbContext context)
        {
            _context = context;
            
        }

        public System.Security.Claims.ClaimsPrincipal GetUser()
        {
            return User;
        }

        [HttpPost("Create"),Authorize]
        public async Task<ActionResult> CreateOrder([Bind("Id, OrderTitle")]Order request, System.Security.Claims.ClaimsPrincipal user)
        {
            try
            {
                Order order = new Order();
                order.OrderTitle = request.OrderTitle;
                order.Creatorname = user?.Identity?.Name;
                order.CreatedDate = DateTime.Now;
                order.OrderStatusId = 1;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost,Authorize(Roles ="Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            return Ok();
        }
    }
}
