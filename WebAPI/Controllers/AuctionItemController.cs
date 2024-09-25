using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DbContexts;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuctionItemController : ControllerBase
    {
        private readonly DataContext context;
        public AuctionItemController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var auctionItems = await context.AuctionItems.Include(b => b.Bids).ToListAsync();
                if (auctionItems.Count == 0)
                {
                    return NotFound("Auction items not available");
                }
                return Ok(auctionItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var auctionItem = await context.AuctionItems.Include(b => b.Bids).FirstOrDefaultAsync(x => x.Id == id);
                if (auctionItem == null)
                {
                    return NotFound($"Auction item with id {id} not found");
                }
                return Ok(auctionItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(AuctionItem auctionItem)
        {
            try
            {
                context.Add(auctionItem);
                await context.SaveChangesAsync();
                return Ok("Auction item created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AuctionItem auctionItem)
        {
            if (auctionItem == null || auctionItem.Id == 0)
                return BadRequest("Auction Item invalid");
            var existingAuctionItem = await context.AuctionItems.FirstOrDefaultAsync(x => x.Id == id);
            if (existingAuctionItem == null)
                return NotFound($"Auction item with id {id} not found");
            try
            {
                existingAuctionItem.Name = auctionItem.Name;
                existingAuctionItem.Description = auctionItem.Description;
                existingAuctionItem.StartingPrice = auctionItem.StartingPrice;
                existingAuctionItem.CurrentPrice = auctionItem.CurrentPrice;
                existingAuctionItem.EndTime = auctionItem.EndTime;
                existingAuctionItem.UpdatedBy = auctionItem.UpdatedBy;
                existingAuctionItem.UpdatedAt = auctionItem.UpdatedAt;

                await context.SaveChangesAsync();
                return Ok(existingAuctionItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var auctionItem = await context.AuctionItems.FindAsync(id);
                if (auctionItem == null)
                {
                    return NotFound($"Auction item with id {id} not found");
                }
                context.AuctionItems.Remove(auctionItem);
                await context.SaveChangesAsync();
                return Ok("Auction item deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
