var builder = WebApplication.CreateBuilder(args);
// Program.cs


// Add services to the container
builder.Services.AddDistributedMemoryCache(); // Voor sessie-opslag
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Voor productie
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();  // Belangrijk: moet na UseRouting() en voor MapRazorPages()
app.MapRazorPages();

app.Run();

