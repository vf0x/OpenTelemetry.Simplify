using OpenTelemetry.Simplify.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain
{
    /// <summary>
    /// Базовый класс для метрики, упрощающий реализацию ICustomMetric.
    /// </summary>
    public abstract class CustomMetricBase : ICustomMetric
    {
        public string Name { get; }
        public string? Description { get; }

        protected CustomMetricBase(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public abstract void Register(Meter meter);
    }
}
