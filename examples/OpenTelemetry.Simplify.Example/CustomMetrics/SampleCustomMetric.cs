using OpenTelemetry.Simplify.Domain.Interfaces;
using OpenTelemetry.Simplify.Example.Sample.Interfaces;
using System.Diagnostics.Metrics;

namespace OpenTelemetry.Simplify.Example.Sample.CustomMetrics
{
    public class SampleCustomMetric : ICustomMetric
    {
        private readonly DateTimeOffset _startTime;
        public SampleCustomMetric()
        {
            _startTime = DateTimeOffset.UtcNow;
        }
        public void Register(Meter meter)
        {
            meter.CreateObservableGauge<double>(
                name: "test_uptime_seconds_test",
                observeValues: () =>
                {
                    double uptime = (DateTimeOffset.UtcNow - _startTime).TotalSeconds;
                    return new[] { new Measurement<double>(uptime) };
                },
                unit: "seconds",
                description: "Description");
        }
    }
}
