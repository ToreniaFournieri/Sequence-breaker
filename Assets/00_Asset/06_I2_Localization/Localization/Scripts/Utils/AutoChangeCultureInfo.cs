using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{

    public class AutoChangeCultureInfo : MonoBehaviour
    {
        public void Start()
        {
            LocalizationManager.EnableChangingCultureInfo(true);
        }
    }
}