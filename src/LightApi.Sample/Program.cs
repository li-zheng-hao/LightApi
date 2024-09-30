using LightApi.Infra;
using LightApi.Infra.OpenTelemetry;
using LightApi.Mongo;
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

Log.Logger=new LoggerConfiguration()
    .Enrich.WithProperty("System","LightApiSample")
    .WriteTo.Console()
    // .WriteTo.Seq("")
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddLightApiSetup(it =>
{
    
});
builder.Services.AddMongoSetup("test", Environment.GetEnvironmentVariable("APP_MONGO_CONNECTIONSTRING"));
builder.Host.AddAutofacSetup();

// builder.Services.AddOpenTelemetry()
//     .ConfigureResource(r => r.AddService(LightApiSource.SourceName))
//     .WithTracing(tracing =>
//     {
//         tracing.AddSource(LightApiSource.SourceName);
//         tracing.SetSampler(new AlwaysOnSampler());
//         tracing.AddOtlpExporter(opt =>
//         {
//             opt.Endpoint = new Uri("/ingest/otlp/v1/traces");
//             opt.Protocol = OtlpExportProtocol.HttpProtobuf;
//         });
//     });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseInfrastructure();

app.UseAuthorization();

app.MapControllers();

app.Run();