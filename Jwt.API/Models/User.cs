namespace Jwt.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string username { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        public string role { get; set; }
        //navigation props
        public virtual ICollection<Order> orders { get; set; }

    }
}
