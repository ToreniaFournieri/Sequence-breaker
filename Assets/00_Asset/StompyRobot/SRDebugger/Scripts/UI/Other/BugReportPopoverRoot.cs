using _00_Asset.StompyRobot.SRF.Scripts.Components;
using UnityEngine;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Other
{
    public class BugReportPopoverRoot : SRMonoBehaviourEx
    {
        [RequiredField] public CanvasGroup CanvasGroup;

        [RequiredField] public RectTransform Container;
    }
}
