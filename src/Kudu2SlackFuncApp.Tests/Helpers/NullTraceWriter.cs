using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace Kudu2SlackFuncApp.Tests.Helpers
{
    internal class NullTraceWriter : TraceWriter
    {
        public NullTraceWriter(TraceLevel level) : base(level)
        {
        }

        public override void Trace(TraceEvent traceEvent)
        {
        }
    }
}