using OpenTelemetry.Simplify.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain.Options
{
    /// <summary>
    /// Настройки трейсов.
    /// </summary>
    public class TracingOptions
    {
        public TraceExporterType Exporter { get; set; } = TraceExporterType.Console;

        internal bool AspNetCoreTraces { get; private set; } = false;
        internal bool HttpClientTraces { get; private set; } = false;

        /// <summary>
        /// Включить трассировку входящих HTTP-запросов (AspNetCoreInstrumentation).
        /// </summary>
        public TracingOptions AddRequestTraces()
        {
            this.AspNetCoreTraces = true;
            return this;
        }

        /// <summary>
        /// Включить трассировку исходящих HTTP-запросов (HttpClientInstrumentation).
        /// </summary>
        public TracingOptions AddHttpClientTraces()
        {
            this.HttpClientTraces = true;
            return this;
        }

        /// <summary>
        /// Установить экспортер.
        /// </summary>
        public TracingOptions UseExporter(TraceExporterType exporter)
        {
            this.Exporter = exporter;
            return this;
        }
    }
}
