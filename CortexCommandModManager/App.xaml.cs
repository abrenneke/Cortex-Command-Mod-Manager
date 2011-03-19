using System.Windows;
using CortexCommandModManager.MVVM;

namespace CortexCommandModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Global Global;

        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Global = new Global(this);
            Global.Start();
        }
    }
}
