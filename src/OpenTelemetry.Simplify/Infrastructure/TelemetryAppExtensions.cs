using OpenTelemetry.Simplify.Domain.Interfaces;
using OpenTelemetry.Simplify.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Infrastructure
{
    public static class TelemetryAppExtensions
    {
        /// <summary>
        /// Регистрирует Prometheus-сервер middleware на пути /metrics по умолчанию.
        /// Вызывается вручную пользователем после builder.Build().
        /// </summary>
        public static IApplicationBuilder UseEubankPrometheusScrapingEndpoint(this IApplicationBuilder app)
        {
            // По сути обёртка над встроенным методом
            return app.UseOpenTelemetryPrometheusScrapingEndpoint();
        }

        /// <summary>
        /// Регистрирует кастомные метрики после запуска приложения.
        /// Позволяет регистрировать метрики с зависимостями через ICustomMetric.Register.
        /// </summary>
        public static void RegisterCustomMetrics(this WebApplication app, params ICustomMetric[] metrics)
        {
            using var scope = app.Services.CreateScope();
            var meter = scope.ServiceProvider.GetRequiredService<IMeterAccessor>().Meter;

            foreach (var metric in metrics)
            {
                metric.Register(meter);
            }
        }
    }
}
