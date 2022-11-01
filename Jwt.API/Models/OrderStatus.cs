namespace Jwt.API.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        //navigation props
        public virtual ICollection<Order> Orders { get; set; }
    }
}
