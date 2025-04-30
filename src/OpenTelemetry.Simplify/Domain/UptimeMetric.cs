using OpenTelemetry.Simplify.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain
{
    public class UptimeMetric : ICustomMetric
    {
        private readonly DateTimeOffset _startTime;

        public UptimeMetric()
        {
            _startTime = DateTimeOffset.UtcNow;
        }

        public void Register(Meter meter)
        {
            meter.CreateObservableGauge<double>(
                name: "service_uptime_seconds",
                observeValues: () =>
                {
                    var uptime = (DateTimeOffset.UtcNow - _startTime).TotalSeconds;
                    return new[] { new Measurement<double>(uptime) };
                },
                unit: "seconds",
                description: "Время работы сервиса в секундах."
            );
        }
    }
}
