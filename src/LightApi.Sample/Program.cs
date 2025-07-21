using LightApi.Infra;
using LightApi.Infra.Authorize.CustomCookie;
using LightApi.Infra.OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomCookieAuth(i => { });
builder.Services.AddLightApiSetup(it => { });
builder.Host.AddAutofacSetup("LightApi");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseInfrastructure();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
