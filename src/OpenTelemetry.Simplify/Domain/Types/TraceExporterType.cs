using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain.Types
{
    /// <summary>
    /// Тип экспортера для трейсов (OTLP, Jaeger, ...).
    /// </summary>
    public enum TraceExporterType
    {
        OTLP,
        Jaeger,
        Console
    }
}
