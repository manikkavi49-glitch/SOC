var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllersWithViews();

// 2. HttpClient එක Named Client එකක් ලෙස සැකසීම (KmcApi)
builder.Services.AddHttpClient("KmcApi", client =>
{
    // API එක රන් වෙන Port එක 7108 ද බලන්න
    client.BaseAddress = new Uri("https://localhost:7108/"); // Swagger එකේ තියෙන Port එකම මෙතනට දෙන්න
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Localhost වලදී SSL certificate validation එක bypass කිරීම
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // සාමාන්‍යයෙන් MVC වලදී මෙය භාවිතා වේ

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();