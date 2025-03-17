using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using susamQr.Models;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace susamQr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext appContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            this.appContext = appDbContext;
        }
        public IActionResult Index()
        {
            ViewData["SiteLogo"] = appContext.Settings.FirstOrDefault(s => s.Name == "logo")?.Value;
            ViewData["SiteDesc"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoDesc")?.Value;
            ViewData["SiteKeywords"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoKeywords")?.Value;
            ViewData["og:image"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:image")?.Value;
            ViewData["og:site_name"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:site_name")?.Value;
            ViewData["og:title"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:title")?.Value;
            ViewData["mainVideo"] = appContext.Settings.FirstOrDefault(s => s.Name == "mainVideo")?.Value;
            ViewData["instagramLink"] = appContext.Settings.FirstOrDefault(s => s.Name == "instagramLink")?.Value;
            ViewData["WptelefonNo"] = appContext.Settings.FirstOrDefault(s => s.Name == "WptelefonNo")?.Value;
            ViewData["googleMapUrl"] = appContext.Settings.FirstOrDefault(s => s.Name == "googleMapUrl")?.Value;
            ViewData["categories"] = appContext.Categories.ToList();
            ViewData["highProducts"] = appContext.Products.Where(x => x.IsHighlight).ToList();
            ViewData["slider-kapak"] = appContext.Sliders.Where(x => x.Title == "main").Select(x => x.ImageUrl).FirstOrDefault();

            return View();
        }

        [HttpGet("main")]
        public IActionResult Main()
        {
            return RedirectToAction("Index");
        }


        [HttpGet("kategori/{slug}")]
        public IActionResult CategoryDetail(string slug)
        {
            ViewData["SiteLogo"] = appContext.Settings.FirstOrDefault(s => s.Name == "logo")?.Value;
            ViewData["SiteDesc"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoDesc")?.Value;
            ViewData["SiteKeywords"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoKeywords")?.Value;
            ViewData["og:image"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:image")?.Value;
            ViewData["og:site_name"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:site_name")?.Value;
            ViewData["og:title"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:title")?.Value;
            ViewData["mainVideo"] = appContext.Settings.FirstOrDefault(s => s.Name == "mainVideo")?.Value;
            ViewData["instagramLink"] = appContext.Settings.FirstOrDefault(s => s.Name == "instagramLink")?.Value;
            ViewData["WptelefonNo"] = appContext.Settings.FirstOrDefault(s => s.Name == "WptelefonNo")?.Value;
            ViewData["googleMapUrl"] = appContext.Settings.FirstOrDefault(s => s.Name == "googleMapUrl")?.Value;
            ViewData["categories"] = appContext.Categories.ToList();
            ViewData["highProducts"] = appContext.Products.Where(x => x.IsHighlight).ToList();
            ViewData["slider-kapak"] = appContext.Sliders.Where(x => x.Title == "main").Select(x => x.ImageUrl).FirstOrDefault();
            var category = appContext.Categories.Include(x => x.Products).FirstOrDefault(c => c.Slug == slug);
            if (category == null)
            {
                return NotFound();
            }



            return View(category);
        }
		[HttpGet("/client/assets/css/style.css")]
		public IActionResult NoCss()
        {
            return View();
        }

		[HttpGet("Search")]
        public IActionResult Search(string q)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 3)
            {
                return Json(new List<object>());
            }

            var products = appContext.Products
                .Where(p => p.Name.Contains(q) || p.Description.Contains(q))
                .Select(p => new
                {
                    name = p.Name,
                    slug = p.Slug,
                    imageUrl = p.ImageUrl,
                    price = p.Price
                })
                .Take(10)
                .ToList();

            return Json(products);
        }
        [HttpGet("urun/{slug}")]
        public IActionResult ProductDetail(string slug)
        {
            ViewData["SiteLogo"] = appContext.Settings.FirstOrDefault(s => s.Name == "logo")?.Value;
            ViewData["SiteDesc"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoDesc")?.Value;
            ViewData["SiteKeywords"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoKeywords")?.Value;
            ViewData["og:image"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:image")?.Value;
            ViewData["og:site_name"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:site_name")?.Value;
            ViewData["og:title"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:title")?.Value;
            ViewData["mainVideo"] = appContext.Settings.FirstOrDefault(s => s.Name == "mainVideo")?.Value;
            ViewData["instagramLink"] = appContext.Settings.FirstOrDefault(s => s.Name == "instagramLink")?.Value;
            ViewData["WptelefonNo"] = appContext.Settings.FirstOrDefault(s => s.Name == "WptelefonNo")?.Value;
            ViewData["googleMapUrl"] = appContext.Settings.FirstOrDefault(s => s.Name == "googleMapUrl")?.Value;
            ViewData["categories"] = appContext.Categories.ToList();
            ViewData["highProducts"] = appContext.Products.Where(x => x.IsHighlight).ToList();
            ViewData["slider-kapak"] = appContext.Sliders.Where(x => x.Title == "main").Select(x => x.ImageUrl).FirstOrDefault();
            var product = appContext.Products.Include(x => x.Category).FirstOrDefault(c => c.Slug == slug);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet("secim")]
        public IActionResult Pages()
        {
            ViewData["SiteLogo"] = appContext.Settings.FirstOrDefault(s => s.Name == "logo")?.Value;
            ViewData["SiteDesc"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoDesc")?.Value;
            ViewData["SiteKeywords"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoKeywords")?.Value;
            ViewData["og:image"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:image")?.Value;
            ViewData["og:site_name"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:site_name")?.Value;
            ViewData["og:title"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:title")?.Value;
            ViewData["mainVideo"] = appContext.Settings.FirstOrDefault(s => s.Name == "mainVideo")?.Value;
            ViewData["instagramLink"] = appContext.Settings.FirstOrDefault(s => s.Name == "instagramLink")?.Value;
            ViewData["WptelefonNo"] = appContext.Settings.FirstOrDefault(s => s.Name == "WptelefonNo")?.Value;
            ViewData["googleMapUrl"] = appContext.Settings.FirstOrDefault(s => s.Name == "googleMapUrl")?.Value;
            ViewData["categories"] = appContext.Categories.ToList();
            ViewData["highProducts"] = appContext.Products.Where(x => x.IsHighlight).ToList();
            ViewData["slider-kapak"] = appContext.Sliders.Where(x => x.Title == "main").Select(x => x.ImageUrl).FirstOrDefault();
            return View();

        }

        [HttpGet("kategoriler")]
        public IActionResult Categories()
        {
            ViewData["SiteLogo"] = appContext.Settings.FirstOrDefault(s => s.Name == "logo")?.Value;
            ViewData["SiteDesc"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoDesc")?.Value;
            ViewData["SiteKeywords"] = appContext.Settings.FirstOrDefault(s => s.Name == "seoKeywords")?.Value;
            ViewData["og:image"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:image")?.Value;
            ViewData["og:site_name"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:site_name")?.Value;
            ViewData["og:title"] = appContext.Settings.FirstOrDefault(s => s.Name == "og:title")?.Value;
            ViewData["mainVideo"] = appContext.Settings.FirstOrDefault(s => s.Name == "mainVideo")?.Value;
            ViewData["instagramLink"] = appContext.Settings.FirstOrDefault(s => s.Name == "instagramLink")?.Value;
            ViewData["WptelefonNo"] = appContext.Settings.FirstOrDefault(s => s.Name == "WptelefonNo")?.Value;
            ViewData["googleMapUrl"] = appContext.Settings.FirstOrDefault(s => s.Name == "googleMapUrl")?.Value;
            ViewData["categories"] = appContext.Categories.Include(x => x.Products).ToList();
            ViewData["highProducts"] = appContext.Products.Where(x => x.IsHighlight).ToList();
            return View();
        }
        [HttpGet("api/settings")]
        public IActionResult GetSettings()
        {
            ViewData["instagramLink"] = appContext.Settings.FirstOrDefault(s => s.Name == "instagramLink")?.Value;
            ViewData["WptelefonNo"] = appContext.Settings.FirstOrDefault(s => s.Name == "WptelefonNo")?.Value;

            return Json(new
            {
                Instagram = ViewData["instagramLink"]?.ToString(),
                WhatsApp = ViewData["WptelefonNo"]?.ToString()
            });
        }
        [HttpGet("iletisim")]
        public IActionResult Contact()
        {
            ViewData["slider-kapak"] = appContext.Sliders.Where(x => x.Title == "main").Select(x => x.ImageUrl).FirstOrDefault();

            ViewData["instagramLink"] = appContext.Settings.FirstOrDefault(s => s.Name == "instagramLink")?.Value;
            ViewData["WptelefonNo"] = appContext.Settings.FirstOrDefault(s => s.Name == "WptelefonNo")?.Value;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

