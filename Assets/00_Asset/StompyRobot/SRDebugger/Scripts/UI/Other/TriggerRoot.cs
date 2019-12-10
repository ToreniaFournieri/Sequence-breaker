using _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Controls;
using _00_Asset.StompyRobot.SRF.Scripts.Components;
using _00_Asset.StompyRobot.SRF.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Other
{
    public class TriggerRoot : SRMonoBehaviourEx
    {
        [RequiredField] public Canvas Canvas;

        [RequiredField] public LongPressButton TapHoldButton;

        [RequiredField] public RectTransform TriggerTransform;

        [RequiredField] [FormerlySerializedAs("TriggerButton")] public MultiTapButton TripleTapButton;
    }
}
