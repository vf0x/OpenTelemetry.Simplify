using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс для кастомной метрики, регистрируемой в Meter.
    /// </summary>
    public interface ICustomMetric
    {
        /// <summary>
        /// Вызывается библиотекой при инициализации, когда Meter готов.
        /// </summary>
        void Register(Meter meter);
    }
}
