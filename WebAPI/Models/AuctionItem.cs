using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class AuctionItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal StartingPrice { get; set; }
        [Required]
        public decimal CurrentPrice { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string ImageFileName { get; set; }
        [Required]
        public string UpdatedBy { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public List<Bid> Bids { get; set; }
    }
}
