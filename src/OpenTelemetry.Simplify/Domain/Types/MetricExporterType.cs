using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain.Types
{
    /// <summary>
    /// Тип экспортера для метрик (Prometheus, OTLP, ...)
    /// </summary>
    public enum MetricExporterType
    {
        Prometheus,
        OTLP,
        Console
    }
}
