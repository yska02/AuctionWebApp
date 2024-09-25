using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AuctionItemViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Item Name")]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DisplayName("Starting Price")]
        public decimal StartingPrice { get; set; }
        [Required]
        [DisplayName("Current Price")]
        public decimal CurrentPrice { get; set; }
        [Required]
        [DisplayName("End Auction Time")]
        public DateTime EndTime { get; set; }
        [Required]
        [DisplayName("Image")]
        public IFormFile Image { get; set; }
        [Required]
        public string ImageFileName { get; set; }
        [Required]
        public string UpdatedBy { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [DisplayName("Bid Amount")]
        public decimal BidAmount { get; set; }
        public List<BidViewModel> Bids { get; set; }
    }
}
