using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._04_Timeline_Tab.Log.BattleLog
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