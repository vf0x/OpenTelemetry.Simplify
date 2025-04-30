using OpenTelemetry.Simplify.Interfaces;
using System.Diagnostics.Metrics;

namespace OpenTelemetry.Simplify.Example.Sample.Middleware
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Counter<long> _counter;

        public MetricsMiddleware(RequestDelegate next, IMeterAccessor meterAccessor)
        {
            _next = next;
            var meter = meterAccessor.Meter;
            _counter = meter.CreateCounter<long>("custom_middleware_requests_total", "req", "asd");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _counter.Add(1);
            await _next(context);
        }
    }
}
