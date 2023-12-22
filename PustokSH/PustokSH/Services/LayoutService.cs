using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.ViewModels;
using static System.Net.WebRequestMethods;

namespace PustokSH.Services
{
    public class LayoutService
    {
        
            private readonly AppDbContext _context;
            private readonly IHttpContextAccessor _http;

        public LayoutService(AppDbContext context, IHttpContextAccessor http)
            {
                _context = context;
                _http = http;
            }
            public async Task<Dictionary<string, string>> GetSettingsAsync()
            {
                Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

                return settings;
            }

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (_http.HttpContext.Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(_http.HttpContext.Request.Cookies["Basket"]);

                foreach (var cookie in cookies)
                {
                    Book book = await _context.Books
                        .Where(x=>x.IsDeleted == false)
                        .Include(p => p.bookImages.Where(pi => pi.IsPrimary == true))
                        .FirstOrDefaultAsync(p => p.Id == cookie.Id);

                    if (book is not null)
                    {
                        BasketItemVM item = new BasketItemVM
                        {
                            Id = book.Id,
                            Product = book.Name,
                            Img = book.bookImages.FirstOrDefault().Image,
                            Price = book.SalePrice-book.Discount,
                            Quantity = cookie.Count,
                            Total = (book.SalePrice - book.Discount) * cookie.Count
                        };
                        items.Add(item);
                    }
                }
            }

            return items;
        }
    }
}
