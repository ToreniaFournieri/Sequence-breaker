using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._06_Timeline.BattleLogView
{
    [System.Serializable]
    public sealed class IconSet : ScriptableObject
    {
        public GameObject iconMask;
        public Image unitIcon;
        public Image hPBar;
        public Image shieldBar;
    }
}