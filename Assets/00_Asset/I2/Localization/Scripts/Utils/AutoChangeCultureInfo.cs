using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset.I2.Localization.Scripts.Utils
{

    public class AutoChangeCultureInfo : MonoBehaviour
    {
        public void Start()
        {
            LocalizationManager.EnableChangingCultureInfo(true);
        }
    }
}