using System;
using System.Diagnostics;
using System.Timers;
using Castle.Core.Logging;
using Statr.Client;

namespace Statr.Monitor
{
    public class Collector : IDisposable
    {
        private Timer timer;

        private PerformanceCounter performanceCounter;

        private readonly IStatrClient client;

        public Collector()
        {
            Logger = NullLogger.Instance;

            client = new StatrClient("localhost");
            InitializePerformanceCounter();
        }

        private void InitializePerformanceCounter()
        {
            performanceCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };

            // pull the first value, will always be zero
            performanceCounter.NextValue();
        }

        public ILogger Logger { get; set; }

        public void StartCollecting()
        {
            Logger.Info("Starting collector");

            timer = new Timer
            {
                Interval = TimeSpan.FromSeconds(1).TotalMilliseconds
            };
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var tick = performanceCounter.NextValue();
            client.Gauge("stats.cpu.processor.time.total", tick);
        }

        public void Dispose()
        {
            Logger.Info("Stopping collector");

            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            if (client != null)
            {
                client.Dispose();
            }
        }
    }
}
