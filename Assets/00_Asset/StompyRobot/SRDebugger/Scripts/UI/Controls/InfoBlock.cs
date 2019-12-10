using _00_Asset.StompyRobot.SRF.Scripts.Components;
using UnityEngine.UI;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Controls
{
    public class InfoBlock : SRMonoBehaviourEx
    {
        [RequiredField] public Text Content;

        [RequiredField] public Text Title;
    }
}
