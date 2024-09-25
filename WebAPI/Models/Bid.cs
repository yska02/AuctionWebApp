using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class Bid
    {
        [Key]
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
        [ForeignKey("AuctionItem")]
        public int AuctionItemId { get; set; }
    }
}
