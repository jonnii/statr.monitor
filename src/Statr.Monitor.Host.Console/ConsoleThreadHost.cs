using System;
using System.Threading;

namespace Statr.Monitor.Host.Console
{
    public class ConsoleThreadHost : IDisposable
    {
        private readonly Thread hostThread;

        private MonitorApplication serverApplication;

        public ConsoleThreadHost()
        {
            hostThread = new Thread(RunStatrServer);
            hostThread.Start();
        }

        private void RunStatrServer()
        {
            serverApplication = new MonitorApplication();
            serverApplication.Initialize();
        }

        public void Dispose()
        {
            serverApplication.Dispose();
        }
    }
}