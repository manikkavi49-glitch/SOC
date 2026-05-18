var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles(); // index.html වගේ files මුලින්ම පෙන්වන්න
app.UseStaticFiles();  // wwwroot එකේ තියෙන files පාවිච්චි කරන්න

app.Run();