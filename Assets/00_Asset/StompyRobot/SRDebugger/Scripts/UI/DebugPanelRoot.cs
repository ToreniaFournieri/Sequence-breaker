using _00_Asset.StompyRobot.SRDebugger.Scripts.Services;
using _00_Asset.StompyRobot.SRF.Scripts.Components;
using _00_Asset.StompyRobot.SRF.Scripts.Service;
using UnityEngine;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI
{
    public class DebugPanelRoot : SRMonoBehaviourEx
    {
        [RequiredField] public Canvas Canvas;

        [RequiredField] public CanvasGroup CanvasGroup;

        [RequiredField] public DebuggerTabController TabController;

        public void Close()
        {
            if (Settings.Instance.UnloadOnClose)
            {
                SRServiceManager.GetService<IDebugService>().DestroyDebugPanel();
            }
            else
            {
                SRServiceManager.GetService<IDebugService>().HideDebugPanel();
            }
        }

        public void CloseAndDestroy()
        {
            SRServiceManager.GetService<IDebugService>().DestroyDebugPanel();
        }
    }
}
