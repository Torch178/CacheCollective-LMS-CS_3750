using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
var builder = WebApplication.CreateBuilder(args);

// Constants
var LOGIN_COOKIE_TIMEOUT = TimeSpan.FromDays(1);

// Add services to the container.

builder.Services.AddRazorPages();

// For claim-based authorization
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("ClaimBasedSchema").AddCookie("ClaimBasedSchema", options =>
{
    options.LoginPath = "/Users/Login";
    options.LogoutPath = "/Users/Logout";
    options.Cookie.HttpOnly = true;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = LOGIN_COOKIE_TIMEOUT;
});

// Add session services for the IsInstructor session field
builder.Services.AddSession(options =>
{
    options.IdleTimeout = LOGIN_COOKIE_TIMEOUT; // Set session timeout
    options.Cookie.HttpOnly = true; // Make the cookie accessible only via HTTP
    options.Cookie.IsEssential = true; // Required for session to work
});

builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RazorPagesMovieContext") ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found.")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() == false)
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

//api controller route for collecting payment data post checkout from stripe
/* ***! - Fix later - !*** controller routing not working properly, either no Json is sent to controller, or Routing
 * on normal Razorpages malfunctions, implementing workaround,by sending static data to success page post-checkout
 * might get back to this later
app.MapControllerRoute(
    name: "WebHook",
    pattern: "WebHook/RecieveRequest");
*/
app.MapRazorPages();

app.Run();