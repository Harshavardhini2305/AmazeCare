// ─────────────────────────────────────────────────────────────
// AmazeCare.MVC/Program.cs
// MVC Frontend Setup
// ─────────────────────────────────────────────────────────────
using AmazeCare.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ───────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── SESSION ───────────────────────────────────────────────────
// Session stores JWT token after login
// Patient/Doctor/Admin never sees the token
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".AmazeCare.Session";
});

builder.Services.AddHttpContextAccessor();

// ── HTTP CLIENT ───────────────────────────────────────────────
// HttpClient calls backend API (AmazeCare.API)
// Base URL read from appsettings.json
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// ── REGISTER SERVICES ─────────────────────────────────────────
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// ── MIDDLEWARE PIPELINE ───────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();        // serves wwwroot files (CSS, JS, images)
app.UseRouting();
app.UseSession();            // must be before MapControllerRoute
app.UseAuthorization();

// ── ROUTES ────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
