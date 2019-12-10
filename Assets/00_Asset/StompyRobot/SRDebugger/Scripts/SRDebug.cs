using _00_Asset.StompyRobot.SRDebugger.Scripts.Services;
using _00_Asset.StompyRobot.SRF.Scripts.Service;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts
{
    public static class SRDebug
    {
        public const string Version = VersionInfo.Version;

        public static IDebugService Instance
        {
            get { return SRServiceManager.GetService<IDebugService>(); }
        }

        public static void Init()
        {
            // Initialize console if it hasn't already initialized.
            SRServiceManager.GetService<IConsoleService>();

            // Load the debug service
            SRServiceManager.GetService<IDebugService>();
        }
    }
}
