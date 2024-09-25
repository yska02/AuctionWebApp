using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class BidViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string BidderName { get; set; }
        [Required]
        public decimal BidAmount { get; set; }
        [Required]
        public DateTime BidTime { get; set; }
        [Required]
        public bool IsAutoBid { get; set; }
        [Required]
        [ForeignKey("AuctionItemViewModel")]
        public int AuctionItemId { get; set; }
    }
}
