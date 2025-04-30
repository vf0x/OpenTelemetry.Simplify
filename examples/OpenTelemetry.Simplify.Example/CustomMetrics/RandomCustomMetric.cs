using OpenTelemetry.Simplify.Domain.Interfaces;
using OpenTelemetry.Simplify.Example.Sample.Interfaces;
using System.Diagnostics.Metrics;

namespace OpenTelemetry.Simplify.Example.Sample.CustomMetrics
{
    public class RandomCustomMetric : ICustomMetric
    {
        private readonly IRandomService _service;
        public RandomCustomMetric(IRandomService service) => _service = service;

        public void Register(Meter meter)
        {
            meter.CreateObservableGauge("active_orders",
                () => new Measurement<int>(_service.GetRandomNumber()));
        }
    }
}
