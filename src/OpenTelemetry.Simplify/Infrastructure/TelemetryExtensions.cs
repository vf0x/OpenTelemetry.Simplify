using OpenTelemetry.Simplify.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Infrastructure
{
    public static class TelemetryExtensions
    {
        public static IServiceCollection AddEubankOpenTelemetry(
            this IServiceCollection services,
            System.Action<TelemetryOptions> configure)
        {
            var options = new TelemetryOptions();
            configure(options);

            using var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetService<ILoggerFactory>()
                ?? LoggerFactory.Create(b => b.AddConsole());
            var logger = loggerFactory.CreateLogger("TelemetrySetup");

            TelemetryConfigurator.ConfigureTelemetry(services, options, logger);

            return services;
        }
    }
}
