using Microsoft.EntityFrameworkCore;
using KmcEventApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();

// SQL Server Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ★ CORS Policy - මෙය අනිවාර්යයි Frontend එකෙන් API එකට කතා කරන්න
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ★ වැදගත්ම පේළිය: UseCors සැමවිටම UseAuthorization වලට පෙර තිබිය යුතුය
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();