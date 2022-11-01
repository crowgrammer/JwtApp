namespace Jwt.API.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        //navigation props
        public List<Order> orders { get; set; }
    }
}
