using System.ComponentModel.DataAnnotations;

namespace ShwubbApi.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientType { get; set; }
        public string Contact { get; set; }
    }
}
