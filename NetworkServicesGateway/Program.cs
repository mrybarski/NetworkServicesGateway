using NetworkServicesGateway.Data;
using NetworkServicesGateway.Hubs;
using NetworkServicesGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<NetworkServicesContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".NetworkServicesGateway.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.IsEssential = true;
});
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Gateway}/{action=Index}/{id?}");
    endpoints.MapHub<NetworkUpdatesMonitorHub>("/network-hub");
});

app.Run();
