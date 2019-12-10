using _00_Asset.StompyRobot.SRDebugger.Scripts.Services;
using _00_Asset.StompyRobot.SRF.Scripts.Service;
using UnityEngine;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts
{
    public static class AutoInitialize
    {
        /// <summary>
        /// Initialize the console service before the scene has loaded to catch more of the initialization log.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnLoadBeforeScene()
        {
            if (Settings.Instance.IsEnabled)
            {
                // Initialize console if it hasn't already initialized.
                SRServiceManager.GetService<IConsoleService>();
            }
        }

        /// <summary>
        /// Initialize SRDebugger after the scene has loaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnLoad()
        {
            if (Settings.Instance.IsEnabled)
            {
                SRDebug.Init();
            }
        }
    }
}