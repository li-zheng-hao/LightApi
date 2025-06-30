using Microsoft.EntityFrameworkCore;
using MultiEfCoreDbContextSample.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddInfrastructureEfCoreSqlServer<DemoDbContext>((sp, op) =>
{
    op.UseSqlite( "Data Source=lightapi_sample_data.db;", b => b.MigrationsAssembly("MultiEfCoreDbContextSample"));
}, typeof(EntityInfo));

builder.Services.AddInfrastructureEfCoreSqlServer<DemoDbContext2>((sp, op) =>
{
    op.UseSqlite( "Data Source=lightapi_sample_data2.db;", 
        b => b.MigrationsAssembly("MultiEfCoreDbContextSample"));
}, typeof(EntityInfo2),false);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<DemoDbContext>();
    var context2 = scope.ServiceProvider.GetService<DemoDbContext2>();
    context!.Database.Migrate();
    context2!.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}