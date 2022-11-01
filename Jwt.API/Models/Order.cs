using System.ComponentModel;


namespace Jwt.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderTitle { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        //navigation props
        [DisplayName("Order Status")]
        public int OrderStatusId { get; set; }
        [DisplayName("Created By")]
        public string Creatorname { get; set; } = string.Empty;
        [DisplayName("Modified By")]
        public int? UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual OrderStatus OrderStatus { get; set; } = null!;
    }
}
