using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.DbContexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) 
            : base(options) { }

        public DbSet<AuctionItem> AuctionItems { get; set; }

        public DbSet<Bid> Bids { get; set; }
    }
}
