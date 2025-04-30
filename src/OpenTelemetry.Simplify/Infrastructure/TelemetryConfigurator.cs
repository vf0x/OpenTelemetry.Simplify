using OpenTelemetry.Simplify.Domain.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Extensions.Hosting;
using System.Diagnostics.Metrics;
using OpenTelemetry.Simplify.Domain.Interfaces;
using OpenTelemetry.Simplify.Domain.Types;
using OpenTelemetry.Trace;
using System.Diagnostics;
using OpenTelemetry.Simplify.Domain;
using OpenTelemetry.Simplify.Infrastructure.Metrics;
using OpenTelemetry.Simplify.Interfaces;

namespace OpenTelemetry.Simplify.Infrastructure
{
    internal static class TelemetryConfigurator
    {
        public static void ConfigureTelemetry(
            IServiceCollection services,
            TelemetryOptions options,
            ILogger logger)
        {
            // Получаем имя сервиса
            if(string.IsNullOrWhiteSpace(options.ServiceName))
                throw new ArgumentNullException(nameof(options.ServiceName));

            var serviceName = options.ServiceName;

            var serviceVersion = string.IsNullOrWhiteSpace(options.ServiceVersion)
                ? TelemetryConstants.DefaultServiceVersion
                : options.ServiceVersion;

            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceVersion: serviceVersion);

            if (options.MetricsEnabled)
            {
                ConfigureMetrics(services, options.Metrics, resourceBuilder, logger, serviceName);
            }


            if (options.TracesEnabled)
            {
                ConfigureTraces(services, options.Tracing, resourceBuilder, logger, serviceName);
            }

            logger.LogInformation("[Telemetry] Configured for service: {ServiceName}", serviceName);
        }

        private static void ConfigureMetrics(
            IServiceCollection services,
            MetricsOptions metricsOptions,
            ResourceBuilder resource,
            ILogger logger,
            string serviceName)
        {
            services.AddOpenTelemetry().WithMetrics(builder =>
            {
                // Создаём Meter
                //var meter = new Meter(serviceName + ".Metrics");
                //builder.AddMeter(serviceName + ".Metrics");

                var meterName = serviceName + ".Metrics";
                var meterAccessor = new MeterAccessor(meterName);

                //Регестрируем MeterAccessor
                services.AddSingleton<IMeterAccessor>(meterAccessor);
                services.AddSingleton(meterAccessor.Meter);
                builder.AddMeter(meterName);

                builder.SetResourceBuilder(resource);

                // Получаем Meter
                var meter = meterAccessor.Meter;

                // Авто-инструментация
                if (metricsOptions.AspNetCorMetricsEnabled)
                {
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddHttpClientInstrumentation();
                }

                if (metricsOptions.SystemMetricsEnabled)
                    builder.AddRuntimeInstrumentation();

                if (metricsOptions.UptimeMetricsEnabled)
                {
                    var uptimeMetric = new UptimeMetric();
                    uptimeMetric.Register(meter);
                }

                if (metricsOptions.OracleMetricsEnabled)
                {
                    builder.AddMeter("Oracle.ManagedDataAccess.Core");
                }

                // Кастомные метрики - объекты
                foreach (var metricObj in metricsOptions.CustomMetricInstances)
                {
                    metricObj.Register(meter);
                }

                // Кастомные метрики - типы
                foreach (var metricType in metricsOptions.CustomMetricTypes)
                {
                    var instance = (ICustomMetric)Activator.CreateInstance(metricType)!;
                    instance.Register(meter);
                }

                // Экспортер
                switch (metricsOptions.Exporter)
                {
                    case MetricExporterType.Prometheus:
                        builder.AddPrometheusExporter();
                        logger.LogInformation("Metrics exporter: Prometheus");
                        break;
                    case MetricExporterType.OTLP:
                        builder.AddOtlpExporter();
                        logger.LogInformation("Metrics exporter: OTLP");
                        break;
                    case MetricExporterType.Console:
                        builder.AddConsoleExporter();
                        logger.LogInformation("Metrics exporter: Console");
                        break;
                    default:
                        logger.LogInformation("No metrics exporter specified.");
                        break;
                }
            });

            logger.LogInformation("Metrics configured for service: {ServiceName}", serviceName);
        }

        private static void ConfigureTraces(
            IServiceCollection services,
            TracingOptions tracingOptions,
            ResourceBuilder resource,
            ILogger logger,
            string serviceName)
        {
            services.AddOpenTelemetry().WithTracing(builder =>
            {
                builder.SetResourceBuilder(resource);

                if (tracingOptions.AspNetCoreTraces)
                    builder.AddAspNetCoreInstrumentation();

                if (tracingOptions.HttpClientTraces)
                    builder.AddHttpClientInstrumentation();

                // Экспортер
                switch (tracingOptions.Exporter)
                {
                    case TraceExporterType.Jaeger:
                        builder.AddJaegerExporter();
                        logger.LogInformation("Trace exporter: Jaeger");
                        break;
                    case TraceExporterType.OTLP:
                        builder.AddOtlpExporter();
                        logger.LogInformation("Trace exporter: OTLP");
                        break;
                    case TraceExporterType.Console:
                        builder.AddConsoleExporter();
                        logger.LogInformation("Trace exporter: Console");
                        break;
                    default:
                        logger.LogInformation("No trace exporter specified.");
                        break;
                }
            });

            // Регистрируем ActivitySource
            services.AddSingleton(_ => new ActivitySource(serviceName + ".Traces"));

            logger.LogInformation("Tracing configured for service: {ServiceName}", serviceName);
        }
    }
}
