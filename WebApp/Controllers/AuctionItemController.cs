using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class AuctionItemController : Controller
    {
        private readonly HttpClient _client;
        private readonly Uri _baseAddress = new Uri("http://localhost:5165/api");
        private readonly string _rootImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//uploads");

        public AuctionItemController()
        {
            _client = new HttpClient { BaseAddress = _baseAddress };
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + endpoint);
            string data = "";
            if (response.IsSuccessStatusCode)
                data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(data);
        }

        public async Task<HttpResponseMessage> SendAsync<T>(T model, string endpoint, HttpMethod method)
        {
            string data = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            return method == HttpMethod.Post ? await _client.PostAsync(_client.BaseAddress + endpoint, content) : await _client.PutAsync(_client.BaseAddress + endpoint, content);
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(_rootImagePath, fileName);
                if (!Directory.Exists(_rootImagePath))
                    Directory.CreateDirectory(_rootImagePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                return fileName;
            }
            return null;
        }

        [Authorize(Roles = "Admin, Regular")]
        [HttpGet]
        public async Task<IActionResult> Index(string searchBy, string search, string sortOrder, int pageNumber=1)
        {
            try
            {
                var auctionItems = await GetAsync<List<AuctionItemViewModel>>("/AuctionItem/Get");
                if (!string.IsNullOrEmpty(search))
                    auctionItems = searchBy == "Name" ? auctionItems.Where(x => x.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase) || search == null).ToList() : auctionItems.Where(x => x.Description.Contains(search, StringComparison.OrdinalIgnoreCase) || search == null).ToList();
                ViewData["PriceSortParam"] = string.IsNullOrEmpty(sortOrder) ? "priceDesc" : "";
                switch (sortOrder)
                {
                    case "priceDesc":
                        auctionItems = auctionItems.OrderByDescending(x => x.CurrentPrice).ToList();
                        break;
                    default:
                        auctionItems =auctionItems.OrderBy(x => x.CurrentPrice).ToList();
                        break;
                }
                var paginatedAuctionItems = await PaginatedList<AuctionItemViewModel>.CreateAsync(auctionItems.AsQueryable(), pageNumber, 10);
                return User.IsInRole("Admin") ? View("IndexAdmin", paginatedAuctionItems) : View(paginatedAuctionItems);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Add() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AuctionItemViewModel model)
        {
            try
            {
                model.CurrentPrice = model.StartingPrice;
                model.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UpdatedAt = DateTime.Now;
                model.ImageFileName = await SaveImageAsync(model.Image);

                HttpResponseMessage response = await SendAsync(model, "/AuctionItem/Post", HttpMethod.Post);
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Auction Item added";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View();
        }

        [Authorize(Roles = "Admin, Regular")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var auctionItem = await GetAsync<AuctionItemViewModel>($"/AuctionItem/Get/{id}");
                if (User.IsInRole("Regular"))
                    ViewData["CanUserBid"] = await CanUserBid(id);
                return View(auctionItem);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var auctionItem = await GetAsync<AuctionItemViewModel>($"/AuctionItem/Get/{id}");
                return View(auctionItem);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(AuctionItemViewModel model)
        {
            try
            {
                model.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UpdatedAt = DateTime.Now;
                HttpResponseMessage response = await SendAsync(model, $"/AuctionItem/Put/{model.Id}", HttpMethod.Put);
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Auction Item updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var auctionItem = await GetAsync<AuctionItemViewModel>($"/AuctionItem/Get/{id}");
                return View(auctionItem);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/AuctionItem/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Auction Item deleted";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }

        public async Task<bool> CanUserBid(int auctionItemId)
        {
            var auctionItems = await GetAsync<List<AuctionItemViewModel>>("/AuctionItem/Get");
            var auctionItem = auctionItems.FirstOrDefault(b => b.Id == auctionItemId);
            if (auctionItem.Bids.Any())
            {
                var highestBid = auctionItem.Bids.OrderByDescending(b => b.BidAmount).FirstOrDefault();
                if (highestBid != null && highestBid.BidderName == User.FindFirstValue(ClaimTypes.NameIdentifier));
                    return false;
            }
            return true; 
        }
    }
}
