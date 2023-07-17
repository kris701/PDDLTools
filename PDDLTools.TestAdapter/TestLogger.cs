using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.TestAdapter
{
    public class TestLogger : IMessageLogger
    {
        private IMessageLogger messageLogger;

        int Verbosity { get; set; }

        public TestLogger()
        {
            Verbosity = 0;
        }

        public TestLogger(int verbosity)
        {
            Verbosity = verbosity;
        }

        public void Initialize(IMessageLogger messageLoggerParam)
        {
            messageLogger = messageLoggerParam;
        }

        public void SendErrorMessage(string message)
        {
            SendMessage(TestMessageLevel.Error, message);
        }

        public void SendErrorMessage(string message, Exception ex)
        {

            switch (Verbosity)
            {
                case 0:
                    var type = ex.GetType();
                    SendErrorMessage(string.Format("Exception {0}, {1}", type, message));
                    break;
                default:
                    SendMessage(TestMessageLevel.Error, message);
                    SendErrorMessage(ex.ToString());
                    break;
            }
        }

        public void SendWarningMessage(string message)
        {
            SendMessage(TestMessageLevel.Warning, message);
        }

        public void SendWarningMessage(string message, Exception ex)
        {
            switch (Verbosity)
            {
                case 0:
                    var type = ex.GetType();
                    SendMessage(TestMessageLevel.Warning, string.Format("Exception {0}, {1}", type, message));
                    break;
                default:
                    SendMessage(TestMessageLevel.Warning, message);
                    SendMessage(TestMessageLevel.Warning, ex.ToString());
                    break;
            }
            SendMessage(TestMessageLevel.Warning, message);
        }

        public void SendInformationalMessage(string message)
        {
            SendMessage(TestMessageLevel.Informational, message);
        }

        public void SendDebugMessage(string message)
        {
#if DEBUG
            SendMessage(TestMessageLevel.Informational, message);
#endif
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            if (messageLogger != null)
                messageLogger.SendMessage(testMessageLevel, message);
        }
    }
}
