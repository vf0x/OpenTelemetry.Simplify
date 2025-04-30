using OpenTelemetry.Simplify.Infrastructure;
using OpenTelemetry.Simplify.Example.Sample.CustomMetrics;
using OpenTelemetry.Simplify.Example.Sample.Interfaces;
using OpenTelemetry.Simplify.Example.Sample.Middleware;
using OpenTelemetry.Simplify.Example.Sample.Services;
using Grpc.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
});
builder.Services.AddHttpClient("httpbin", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        UseProxy = false
    };
    }); 

builder.Services.AddGrpcReflection();
builder.Services.AddGrpcHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

builder.Services.AddSimplifyOpenTelemetry(options =>
{
    options.ServiceName = "Sample";
    options.ServiceVersion = "1.0.0";
    options.AddMetrics(metric =>
    {
        metric.AddAspNetCorMetrics()
            .AddSystemMetrics()
            .AddOracleMetrics()
            .AddUptimeMetrics()
            .AddCustomMetric<SampleCustomMetric>()
            .UseExporter(OpenTelemetry.Simplify.Domain.Types.MetricExporterType.Prometheus);
    });
});
builder.Services.AddTransient<IRandomService, RandomService>();

var app = builder.Build();


app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Otel.Sample");
    });
}
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.UseOtelPrometheusScrapingEndpoint();
app.MapGrpcHealthChecksService();
app.MapGrpcReflectionService();
app.UseMiddleware<MetricsMiddleware>();
app.RegisterCustomMetrics(new RandomCustomMetric(app.Services.GetRequiredService<IRandomService>()));
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();
