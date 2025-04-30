using OpenTelemetry.Simplify.Domain.Interfaces;
using OpenTelemetry.Simplify.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain.Options
{
    /// <summary>
    /// Настройки метрик.
    /// </summary>
    public class MetricsOptions
    {
        /// <summary>
        /// Тип экспортера для метрик (Prometheus, OTLP, ...).
        /// </summary>
        public MetricExporterType Exporter { get; set; } = MetricExporterType.Console;

        // Включаем метрики
        internal bool Enabled { get; private set; } = false;

        // Храним, включать ли ASP.NET Core запросы, системные метрики и т.д.
        internal bool AspNetCorMetricsEnabled { get; private set; } = false;
        internal bool SystemMetricsEnabled { get; private set; } = false;
        internal bool UptimeMetricsEnabled { get; private set; } = false;
        internal bool OracleMetricsEnabled { get; private set; } = false;

        // Список кастомных метрик, добавленных как объекты
        internal List<ICustomMetric> CustomMetricInstances { get; } = new();

        // Список типов кастомных метрик, добавленных generic-методом
        internal List<Type> CustomMetricTypes { get; } = new();

        /// <summary>
        /// Включить сбор метрик HTTP-запросов (AspNetCoreInstrumentation для метрик).
        /// </summary>
        public MetricsOptions AddAspNetCorMetrics()
        {
            this.AspNetCorMetricsEnabled = true;
            return this;
        }

        /// <summary>
        /// Включить сбор системных метрик (RuntimeInstrumentation).
        /// </summary>
        public MetricsOptions AddSystemMetrics()
        {
            this.SystemMetricsEnabled = true;
            return this;
        }

        /// <summary>
        /// Включить сбор метрик с Oracle базы данных (Oracle.ManagedDataAccess.Core).
        /// </summary>
        public MetricsOptions AddOracleMetrics()
        {
            this.OracleMetricsEnabled = true;
            return this;
        }


        /// <summary>
        /// Включить сбор системных метрик (RuntimeInstrumentation).
        /// </summary>
        public MetricsOptions AddUptimeMetrics()
        {
            this.UptimeMetricsEnabled = true;
            return this;
        }

        /// <summary>
        /// Установить экспортер.
        /// </summary>
        public MetricsOptions UseExporter(MetricExporterType exporter)
        {
            this.Exporter = exporter;
            return this;
        }

        /// <summary>
        /// Добавить кастомную метрику как объект.
        /// </summary>
        public MetricsOptions AddCustomMetric(ICustomMetric metricInstance)
        {
            CustomMetricInstances.Add(metricInstance);
            return this;
        }

        /// <summary>
        /// Добавить кастомную метрику по типу (generic), 
        /// библиотека сама создаст экземпляр через Activator.
        /// </summary>
        public MetricsOptions AddCustomMetric<T>() where T : ICustomMetric, new()
        {
            CustomMetricTypes.Add(typeof(T));
            return this;
        }
    }
}
