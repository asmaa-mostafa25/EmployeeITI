using EmployeeDep.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session requires a cache and the session services.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".EmployeeDep.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// NOTE: These custom middlewares must run BEFORE app.MapControllerRoute(), because
// MapControllerRoute() registers the endpoint-execution middleware at the position it
// is called. Anything registered with app.UseMiddleware<>() AFTER it only runs for
// requests that don't match any controller action (almost never), so it would never
// really fire. Registering them here, right after UseRouting()/UseSession(), makes sure
// every request (Request Logger, Request Counter, Response Timer, Maintenance check)
// passes through them first.
app.UseRouting();

app.UseSession();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<RequestCounterMiddleware>();
app.UseMiddleware<ResponseTimeMiddleware>();
app.UseMiddleware<MaintenanceMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
