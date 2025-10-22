using System.ComponentModel.DataAnnotations;

namespace ShwubbApi.Models
{
    public class ClientRequest
    {
        [Key]
        public int urlid { get; set; }
    }
}
