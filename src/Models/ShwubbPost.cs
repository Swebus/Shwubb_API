using System.ComponentModel.DataAnnotations;

namespace ShwubbApi.Models
{
    public class ShwubbPost
    {
        [Key]
        public int Postid { get; set; }

        public string Title { get; set; }
        public string? Content { get; set; }
        public string? ImagePath { get; set; }
        public int Userid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ShwubbUser Author { get; set; }
    }
}
