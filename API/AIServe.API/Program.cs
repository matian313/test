using Com.AIServe.Common.Data;
using AIServe.API.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 使用SQLite作为默认数据库
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=AIServe.db"));

builder.Services.AddScoped<HandlerFactory>();
builder.Services.AddScoped<Com.AIServe.Handlers.Reservation.Handlers.ReservationHandler>();
builder.Services.AddScoped<Com.AIServe.Handlers.Setup.Handlers.SetupHandler>();
builder.Services.AddScoped<Com.AIServe.Handlers.Setup.Handlers.LoginHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

await InitializeDatabaseAsync(app);

app.Run();

async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await db.Database.EnsureCreatedAsync();
    logger.LogInformation("数据库已就绪");

    if (!await db.Reservations.AnyAsync())
    {
        await db.Reservations.AddRangeAsync(
            new Com.AIServe.Common.Models.Reservation
            {
                CustomerName = "张三",
                Phone = "13800138001",
                ReservationTime = DateTime.Now.AddDays(1),
                ServiceType = 1,
                Status = 1,
                Remark = "咨询预约",
                CreatedAt = DateTime.Now
            },
            new Com.AIServe.Common.Models.Reservation
            {
                CustomerName = "李四",
                Phone = "13800138002",
                ReservationTime = DateTime.Now.AddDays(2),
                ServiceType = 2,
                Status = 2,
                Remark = "保养服务",
                CreatedAt = DateTime.Now
            }
        );
        await db.SaveChangesAsync();
        logger.LogInformation("已初始化种子数据");
    }
}
