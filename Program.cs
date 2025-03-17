using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Read connection string from appsettings.json
var configuration = builder.Configuration;
string connectionString = configuration.GetConnectionString("MySqlConnection");
// Session'ı ekle
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi 30 dakika
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Configure Entity Framework Core with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
	var path = context.Request.Path.Value;

	// Eğer istek /client/assets/css veya /client/assets/js klasörüne gidiyorsa özel işlem yap
	if (path != null && (path.StartsWith("/client/assets/css/") || path.StartsWith("/client/assets/js/")))
	{
		var referer = context.Request.Headers["Referer"].ToString();

		// Eğer referer boşsa, doğrudan tarayıcıdan açılmasını engelle ve özel mesaj göster
		if (string.IsNullOrEmpty(referer))
		{
			context.Response.StatusCode = 403; // Yasaklandı
			context.Response.ContentType = "text/html";

			await context.Response.WriteAsync(@"
                <html>
                <head>
                    <title>403 - Erisim Engellendi</title>
                    <style>
                        body { font-family: Arial, sans-serif; text-align: center; margin-top: 50px; }
                        h1 { color: red; }
                        p { font-size: 18px; }
                    </style>
                </head>
                <body>
                    <h1>403 - Forbidden</h1>
                    <p>Senin yakaladigin fare kadar...</p>
                    <p>Ana sayfaya git: <a href='/'>Ana Sayfa</a></p>
                </body>
                </html>
            ");
			return;
		}

		// Eğer dosya var ise servise devam et
		var filePath = Path.Combine("wwwroot", path.TrimStart('/'));

		if (File.Exists(filePath))
		{
			var contentType = path.EndsWith(".css") ? "text/css" :
							  path.EndsWith(".js") ? "application/javascript" : "application/octet-stream";

			context.Response.ContentType = contentType;
			await context.Response.SendFileAsync(filePath);
			return;
		}

		// Dosya yoksa 404 Not Found döndür
		context.Response.StatusCode = 404;
		context.Response.ContentType = "text/html";
		await context.Response.WriteAsync("<h1>404 - Dosya Bulunamadı</h1>");
		return;
	}

	await next();
});

app.UseStaticFiles(new StaticFileOptions
{
	OnPrepareResponse = ctx =>
	{
		var referer = ctx.Context.Request.Headers["Referer"].ToString();
		var path = ctx.File.PhysicalPath;

		// Eğer istek direkt tarayıcıdan yapıldıysa engelle, ama site içinden geldiyse izin ver
		if (string.IsNullOrEmpty(referer) && path != null &&
			(path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".jpg") || path.EndsWith(".png")))
		{
			ctx.Context.Response.StatusCode = 403; // Erişimi engelle
			ctx.Context.Response.ContentLength = 0;
			ctx.Context.Response.Body = Stream.Null;
		}
	}
});
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();