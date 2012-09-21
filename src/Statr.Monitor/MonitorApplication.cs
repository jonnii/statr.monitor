using System;
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Statr.Monitor.Installers;

namespace Statr.Monitor
{
    public class MonitorApplication : IDisposable
    {
        public IWindsorContainer Container { get; private set; }

        public void Initialize()
        {
            var root = WhereAmI(GetType().Assembly);
            var configPath = Path.Combine(root, "Configuration/Windsor.xml");

            Container = new WindsorContainer(configPath);

            var installers = new IWindsorInstaller[]
            {
                new InfrastructureInstaller(LogFileName),
                new MonitorInstaller(),
            };

            Container.Install(installers);
        }

        public string LogFileName { get; set; }

        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
        }

        private string WhereAmI(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
