using _00_Asset.StompyRobot.SRF.Scripts.Components;
using UnityEngine.UI;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Other
{
    public class VersionTextBehaviour : SRMonoBehaviourEx
    {
        public string Format = "SRDebugger {0}";

        [RequiredField] public Text Text;

        protected override void Start()
        {
            base.Start();

            Text.text = string.Format(Format, SRDebug.Version);
        }
    }
}
