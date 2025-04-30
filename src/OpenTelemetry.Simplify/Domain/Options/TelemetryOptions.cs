using Microsoft.Extensions.Diagnostics.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain.Options
{
 /// <summary>
    /// Общий класс настроек библиотеки.
    /// </summary>
    public class TelemetryOptions
    {
        /// <summary>
        /// Имя сервиса (микросервиса), для ResourceBuilder.
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// Версия сервиса.
        /// </summary>
        public string? ServiceVersion { get; set; }

        /// <summary>
        /// Настройки метрик.
        /// </summary>
        internal MetricsOptions Metrics { get; } = new MetricsOptions();

        /// <summary>
        /// Настройки трейсов.
        /// </summary>
        internal TracingOptions Tracing { get; } = new TracingOptions();

        /// <summary>
        /// Включаем метрики
        /// </summary>
        internal bool MetricsEnabled { get; set; } = true;

        /// <summary>
        /// Включаем трейсы
        /// </summary>
        internal bool TracesEnabled { get; set; } = false;

        // Fluent-методы (необязательно, но для красоты):

        public TelemetryOptions AddMetrics(System.Action<MetricsOptions> configure)
        {
            MetricsEnabled = true;
            configure?.Invoke(Metrics);
            return this;
        }

        public TelemetryOptions AddTrace(System.Action<TracingOptions> configure)
        {
            TracesEnabled = true;
            configure?.Invoke(Tracing);
            return this;
        }
    }
}
