using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
sealed public class IconSet : ScriptableObject
{
    public GameObject iconMask;
    public Image unitIcon;
    public Image hPBar;
    public Image shieldBar;
}