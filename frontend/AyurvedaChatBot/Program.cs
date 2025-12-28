using AyurBotFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();

// Extension method to launch browser
public static class AppExtensions
{
    public static void LaunchBrowser(this WebApplication app)
    {
        var url = app.Urls.FirstOrDefault(u => u.StartsWith("https://")) ?? app.Urls.FirstOrDefault();
        if (url != null)
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(processStartInfo);
        }
    }
}
