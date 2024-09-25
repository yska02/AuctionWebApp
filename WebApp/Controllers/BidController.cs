using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class BidController : Controller
    {
        private readonly HttpClient _client;
        private readonly Uri _baseAddress = new Uri("http://localhost:5165/api");

        public BidController()
        {
            _client = new HttpClient { BaseAddress = _baseAddress };
        }

        [Authorize(Roles = "Regular")]
        [HttpPost]
        public async Task<IActionResult> PlaceBid(BidViewModel model)
        {
            try
            {
                // Update Auction Item CurrentPrice
                HttpResponseMessage auctionItemResponse = await _client.GetAsync(_client.BaseAddress + $"/AuctionItem/Get/{model.AuctionItemId}");
                var auctionItemContent = await auctionItemResponse.Content.ReadAsStringAsync();
                var auctionItem = JsonConvert.DeserializeObject<AuctionItemViewModel>(auctionItemContent);
                auctionItem.CurrentPrice = model.BidAmount;
                string auctionData = JsonConvert.SerializeObject(auctionItem);
                StringContent auctionContent = new StringContent(auctionData, Encoding.UTF8, "application/json");
                HttpResponseMessage updateAuctionItemResponse = await _client.PutAsync(_client.BaseAddress + $"/AuctionItem/Put/{auctionItem.Id}", auctionContent);

                if (updateAuctionItemResponse.IsSuccessStatusCode)
                {
                    model.BidTime = DateTime.Now;
                    model.BidderName = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    string bidData = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(bidData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Bid/Post", content);

                    if (response.IsSuccessStatusCode)
                        TempData["successMessage"] = "Your bid has been placed!";
                    return RedirectToAction("Details", "AuctionItem", new { id = model.AuctionItemId });
                }
                return RedirectToAction("Index", "AuctionItem");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index", "AuctionItem");
            }
        }
    }
}
