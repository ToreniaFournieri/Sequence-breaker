using _00_Asset.StompyRobot.SRF.Scripts.Components;
using _00_Asset.StompyRobot.SRF.Scripts.Extensions;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Other
{
    public class SetLayerFromSettings : SRMonoBehaviour
    {
        private void Start()
        {
            gameObject.SetLayerRecursive(Settings.Instance.DebugLayer);
        }
    }
}
