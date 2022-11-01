using Jwt.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<List<Order>>> GetAll()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.OrderStatus)
                    .ToListAsync();
                return Ok(orders);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("GetOrder")]
        public async Task<ActionResult<Order>> GetOrder(int? id)
        {
            try
            {
            var order = await _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.OrderStatus)
                    .FirstOrDefaultAsync(y => y.Id == id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("Create"),Authorize]
        public async Task<ActionResult<string>> CreateOrder(string request)
        {
                Order order = new Order();
                order.OrderTitle = request;
                order.Creatorname = User?.Identity?.Name;
                order.CreatedDate = DateTime.Now;
                order.OrderStatusId = 1;

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return Ok($"Your Order I is {order.Id} Please Save it for tracking purposes.");
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("Delete"),Authorize(Roles ="Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
                var order = await _context.Orders.FindAsync(id);
                if(order == null)
                {
                    return NotFound();
                }
            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("GetAvailableStatus"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<OrderStatus>>> Update()
        {
            try
            {
                var status = await _context.orderStatuse.ToListAsync();
                return Ok(status);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("Update"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<String>> Update(int? id, string? newStatus)
        {
            
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound("Order Not Found");
                }
                var status = _context.orderStatuse.FirstOrDefaultAsync(x => x.Status.Equals(newStatus));
                if (status == null)
                {
                    return NotFound("New Status Not Found");
                }
            order.OrderStatusId = status.Result.Id;
                var user = _context.Users.FirstOrDefault(x => x.username.Equals(User.Identity.Name));
                order.UserId = user.Id;
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return Ok(order);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("Track"), AllowAnonymous]
        public async Task<ActionResult<Dictionary<String, String>>> TrackOrder(int? id)
        {
            var order = await _context.Orders.Include(x => x.User).Include(x => x.OrderStatus).FirstOrDefaultAsync(y => y.Id == id);
            if(order == null) { return NotFound("Order Not Found"); }
            Dictionary<String, String> result = new Dictionary<String, String>();
            result.Add("Order ID", order.Id.ToString());
            result.Add("Order Title", order.OrderTitle);
            result.Add("Order Status", order.OrderStatus.Status);
            result.Add("Order Created", order.CreatedDate.ToString());
            result.Add("Order Last Modified By", order.User.username);
            return Ok(result);
        }

    }
}
