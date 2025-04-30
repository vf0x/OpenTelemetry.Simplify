using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Simplify.Interfaces
{
    public interface IMeterAccessor
    {
        Meter Meter { get; }
    }
}
