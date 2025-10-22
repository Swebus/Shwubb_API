using System.ComponentModel.DataAnnotations;

namespace ShwubbApi.Models
{
    public class ShwubbUser
    {
        [Key]
        public int Userid { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }


        public ICollection<ShwubbPost> Posts { get; set; } = new List<ShwubbPost>();
    }
}
