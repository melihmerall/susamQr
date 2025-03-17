using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using susamQr.Models.Entities;

namespace susamQr.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Login işlemi
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null && user.PasswordHash == Models.Entities.User.HashPassword(password))
            {
                HttpContext.Session.SetString("Admin", user.Username);
                return RedirectToAction("Dashboard");
            }
            else
            {
                var isAdmin = _context.Users.Where(x=>x.Username == "susamsokagi@admin.com").FirstOrDefault();
                if(isAdmin == null)
                {
					var adminUser = new User
					{
						Username = "susamsokagi@admin.com",
                        Email = "susamsokagi@admin.com",
						PasswordHash = Models.Entities.User.HashPassword("L4CiXspzSzWrvSg"),
					};
                    _context.Users.Add(adminUser);
                    _context.SaveChanges();
					return RedirectToAction("Dashboard");
				}

			}
			ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("Admin") != null)
            {
                HttpContext.Session.Remove("Admin");
            }
            return RedirectToAction("Login");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            //if (HttpContext.Session.GetString("Admin") == null)
            //    return RedirectToAction("Login");

            var productCount = _context.Products.Count();
            var categoryCount = _context.Categories.Count();

            ViewBag.CategoryCount = categoryCount;
            ViewBag.ProductCount = productCount;


            var settings = _context.Settings.ToList();

            return View(settings);
        }

        // Kategori işlemleri
        [HttpGet("kategori-listesi")]
        public IActionResult Categories()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpGet("kategori-ekle")]
        public async Task<IActionResult> AddCategory()
        {

            return View();
        }

        [HttpPost("kategori-ekle")]
        public async Task<IActionResult> AddCategory(Category category)
        {

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Categories");
        }

        // Ürün işlemleri
        [HttpGet("urun-listesi")]
        public IActionResult Products()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            ViewBag.Categories =  _context.Categories
                                  .Where(c => c.Status) // 🔹 Sadece aktif kategorileri göster
                                  .Select(c => new { c.Id, c.Name })
                                  .ToList();
            return View(products);
        }


        [HttpGet("urun-ekle-")]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Categories = await _context.Categories
                                  .Where(c => c.Status) // 🔹 Sadece aktif kategorileri göster
                                  .Select(c => new { c.Id, c.Name })
                                  .ToListAsync();
            return View();
        }

        [HttpPost("urun-ekle-")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Products");
            }
            return View("Products", _context.Products.Include(p => p.Category).ToList());
        }

        // Slider işlemleri
        [HttpGet("sliders")]
        public IActionResult Sliders()
        {
            var sliders = _context.Sliders.ToList();
            return View(sliders);
        }

        [HttpGet("AddSlider")]
        public IActionResult AddSlider()
        {
            return View();
        }
        // ✅ Yeni Slider Ekleme
        [HttpPost("AddSlider")]
        public async Task<IActionResult> AddSlider(Slider model, IFormFile ImageFile)
        {

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                model.ImageUrl = "/uploads/" + fileName;
            }

            _context.Sliders.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Slider başarıyla eklendi!" });
        }

        // ✅ Slider Güncelleme
        [HttpPost("UpdateSlider")]
        public async Task<IActionResult> UpdateSlider(int id, string title, string description, IFormFile image)
        {
            var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider == null)
            {
                return Json(new { success = false, message = "Slider bulunamadı!" });
            }

            slider.Title = title;
            slider.Description = description;

            if (image != null && image.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                slider.ImageUrl = "/uploads/" + fileName;
            }

            _context.Sliders.Update(slider);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Slider başarıyla güncellendi!" });
        }

        // ✅ Slider Silme
        [HttpPost("DeleteSlider")]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider == null)
            {
                return Json(new { success = false, message = "Slider bulunamadı!" });
            }

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Slider başarıyla silindi!" });
        }
        [HttpPost]
        public async Task<IActionResult> AddSettingWithMedia(string Name, string Value, IFormFile ImageFile, IFormFile VideoFile)
        {
            var setting = new Setting
            {
                Name = Name
            };

            // Eğer resim yüklenmişse kaydet
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/settings", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                setting.Value = "/uploads/settings/" + fileName; // Resmin URL'si
            }

            // Eğer video yüklenmişse kaydet
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var videoFileName = Guid.NewGuid().ToString() + Path.GetExtension(VideoFile.FileName);
                var videoFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/videos", videoFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(videoFilePath));

                using (var stream = new FileStream(videoFilePath, FileMode.Create))
                {
                    await VideoFile.CopyToAsync(stream);
                }

                setting.Value = "/uploads/videos/" + videoFileName; // Videonun URL'si
            }

            // Eğer resim ve video yoksa metin ekle
            if (string.IsNullOrEmpty(setting.Value))
            {
                setting.Value = Value;
            }

            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpPost("UpdateSetting")]
        public async Task<IActionResult> UpdateSetting(int Id, string Name, string Value, IFormFile ImageFile, IFormFile VideoFile)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.Id == Id);
            if (setting == null)
            {
                return Json(new { success = false, message = "Ayar bulunamadı!" });
            }

            setting.Name = Name;

            // Eğer resim yüklenmişse güncelle
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/settings", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                setting.Value = "/uploads/settings/" + fileName; // Resmin URL'sini kaydet
            }

            // Eğer video yüklenmişse güncelle
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var videoFileName = Guid.NewGuid().ToString() + Path.GetExtension(VideoFile.FileName);
                var videoFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/videos", videoFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(videoFilePath));

                using (var stream = new FileStream(videoFilePath, FileMode.Create))
                {
                    await VideoFile.CopyToAsync(stream);
                }

                setting.Value = "/uploads/videos/" + videoFileName; // Videonun URL'sini kaydet
            }

            // Eğer resim veya video yüklenmediyse, sadece metni güncelle
            if (ImageFile == null && VideoFile == null)
            {
                setting.Value = Value;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // Ayarlar işlemleri
        public IActionResult Settings()
        {
            var settings = _context.Settings.ToList();
            return View(settings);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.Id == id);
            if (setting != null)
            {
                _context.Settings.Remove(setting);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Ayar bulunamadı!" });
        }
        [HttpGet]
        public IActionResult AddSetting()
        {
            var settings = _context.Settings.ToList();
            return View(settings);
        }
        [HttpPost]
        public async Task<IActionResult> AddSetting(Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz giriş!" });
            }

            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Ayar başarıyla eklendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSetting(int id, string value)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.Id == id);
            if (setting != null)
            {
                setting.Value = value;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Ayar bulunamadı!" });
        }
        // ✅ Kategoriler - AJAX ile getir
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.Select(c => new { c.Id, c.Name }).ToList();
            return Json(new { data = categories });
        }

		// ✅ Yeni Kategori Ekleme (AJAX)
		[HttpPost]
		public async Task<IActionResult> AddCategoryAjax(Category category, IFormFile ImageFile)
		{
			if (ImageFile != null && ImageFile.Length > 0)
			{
				// 📌 Benzersiz dosya ismi oluştur
				string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
				string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/categories");

				if (!Directory.Exists(uploadPath))
				{
					Directory.CreateDirectory(uploadPath);
				}

				string filePath = Path.Combine(uploadPath, uniqueFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await ImageFile.CopyToAsync(stream);
				}

				category.ImageUrl = "/uploads/categories/" + uniqueFileName; // 🌐 URL olarak kaydet
			}

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			return Json(new { success = true });
		}

		// ✅ Kategori Silme (AJAX)
		[HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return Json(new { success = false, message = "Kategori bulunamadı." });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ✅ Ürünler - AJAX ile getir
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.Include(p => p.Category)
                .Select(p => new { p.Id, p.Name, CategoryName = p.Category.Name, p.Price, p.Status })
                .ToList();

            return Json(new { data = products });
        }

        // ✅ Ürün Ekleme (AJAX)
        [HttpPost]
        public async Task<IActionResult> AddProductAjax(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Ürün eklenemedi." });
        }

        // ✅ Ürün Durumu Güncelleme (Checkbox AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateProductStatus(int id, bool isActive)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return Json(new { success = false, message = "Ürün bulunamadı." });

            product.Status = isActive;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ✅ Slider Listesi - AJAX
        [HttpGet]
        public IActionResult GetSliders()
        {
            var sliders = _context.Sliders.Select(s => new { s.Id, s.Title, s.ImageUrl }).ToList();
            return Json(new { data = sliders });
        }

        // ✅ Slider Ekleme (AJAX)
        [HttpPost]
        public async Task<IActionResult> AddSliderAjax(Slider slider)
        {
            if (ModelState.IsValid)
            {
                _context.Sliders.Add(slider);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Slider eklenemedi." });
        }

        // ✅ Ayarlar - AJAX
        [HttpGet]
        public IActionResult GetSettings()
        {
            var settings = _context.Settings.Select(s => new { s.Id, s.Name, s.Value }).ToList();
            return Json(new { data = settings });
        }

        // ✅ Ayar Güncelleme (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateSettingAjax(Setting setting)
        {
            var existingSetting = _context.Settings.FirstOrDefault(s => s.Id == setting.Id);
            if (existingSetting != null)
            {
                existingSetting.Value = setting.Value;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Ayar güncellenemedi." });
        }
		// ✅ Kategorileri JSON olarak getir
		[HttpGet]
		public IActionResult GetCategoriesAjax()
		{
			var categories = _context.Categories.Select(c => new
			{
				c.Id,
				c.Name,
				c.Description,
				c.Status,
				c.ImageUrl
			}).ToList();

			return Json(new { data = categories });
		}

		// ✅ Kategori Güncelleme (AJAX)
		[HttpPost]
		public async Task<IActionResult> UpdateCategory(int id, string name, string description, bool status)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null) return Json(new { success = false });

			category.Name = name;
			category.Description = description;
			category.Status = status;

			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		// ✅ Kategori Silme (AJAX)
		[HttpPost]
		public async Task<IActionResult> DeleteCategoryAjax(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null) return Json(new { success = false });

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

        [HttpPost]
        public async Task<IActionResult> UpdateCategoryWithImage(int id, string name, string description, bool status, string slug, IFormFile image)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            category.Name = name;
            category.Description = description;
            category.Status = status;
            category.Slug = slug;

            if (image != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                category.ImageUrl = "/uploads/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ✅ Yeni Ürün Ekleme (AJAX)
        [HttpPost("urun-ekle")]
        public async Task<IActionResult> AddProductAjax(Product product, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "/uploads/products/" + uniqueFileName;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // ✅ Ürün Güncelleme (AJAX)
        [HttpPost("urun-guncelle")]
        public async Task<IActionResult> UpdateProduct(int id, string name, string description, decimal price, bool status, bool isHighlight, IFormFile image)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return Json(new { success = false });

            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.Status = status;
            product.IsHighlight = isHighlight;
            if (image != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                product.ImageUrl = "/uploads/products/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ✅ Ürün Silme (AJAX)
        // ✅ Ürün Silme (AJAX)
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return Json(new { success = false, message = "Ürün bulunamadı." });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductWithImage(int id, string name,string desc,string slug, decimal price, bool status, bool isHighlight,int categoryId, IFormFile image)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı!" });
            }

            product.Name = name;
            product.Price = price;
            product.Status = status;
            product.IsHighlight = isHighlight;
            product.CategoryId = categoryId;
            product.Description = desc;
            product.Slug = slug;

            if (image != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                product.ImageUrl = "/uploads/products/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

}
