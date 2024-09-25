using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DbContexts;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BidController : ControllerBase
    {
        private readonly DataContext context;
        public BidController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var bids = await context.Bids.ToListAsync();
                if (bids.Count == 0)
                {
                    return NotFound("Bids not available");
                }
                return Ok(bids);
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
                var bid = await context.Bids.FirstOrDefaultAsync(x => x.Id == id);
                if (bid == null)
                {
                    return NotFound($"Auction item with id {id} not found");
                }
                return Ok(bid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(Bid bid)
        {
            try
            {
                context.Add(bid);
                await context.SaveChangesAsync();
                return Ok("Bid created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
