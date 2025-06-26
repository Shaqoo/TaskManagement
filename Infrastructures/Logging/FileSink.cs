using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Logging
{

    using Serilog.Core;
    using Serilog.Events;

    public class FileSink : ILogEventSink
    {
        private readonly StreamWriter _writer;

        public FileSink(string filePath)
        {
            _writer = new StreamWriter(filePath, append: true) { AutoFlush = true };
        }

        public void Emit(LogEvent logEvent)
        {
            var message = $"{logEvent.Timestamp:u} [{logEvent.Level}] {logEvent.RenderMessage()}";
            if (logEvent.Exception != null)
            {
                message += Environment.NewLine + logEvent.Exception;
            }

            _writer.WriteLine(message);
        }
    }

}
