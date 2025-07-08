using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  
builder.Services.AddControllersWithViews();

// Use Electron.NET integration.  
builder.WebHost.UseElectron(args); // Ensure Electron.NET is installed and referenced.  
builder.Services.AddElectron();

var app = builder.Build();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.  
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}");

await app.StartAsync();
var browerWindowOptions = new BrowserWindowOptions
{
    AutoHideMenuBar = true,
};
await Electron.WindowManager.CreateWindowAsync(browerWindowOptions);
app.WaitForShutdown();
