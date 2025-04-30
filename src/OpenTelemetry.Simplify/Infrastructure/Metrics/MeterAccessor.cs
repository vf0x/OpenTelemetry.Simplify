using OpenTelemetry.Simplify.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Infrastructure.Metrics
{
    internal class MeterAccessor : IMeterAccessor
    {
        public Meter Meter { get; }

        public MeterAccessor(string meterName)
        {
            Meter = new Meter(meterName);
        }
    }
}
