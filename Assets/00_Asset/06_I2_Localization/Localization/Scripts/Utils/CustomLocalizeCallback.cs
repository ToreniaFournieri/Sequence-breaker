using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{
    [AddComponentMenu("I2/Localization/I2 Localize Callback")]
	public class CustomLocalizeCallback : MonoBehaviour
	{
        [FormerlySerializedAs("_OnLocalize")] public UnityEvent onLocalize = new UnityEvent();
		
		public void OnEnable()
		{
            LocalizationManager.OnLocalizeEvent -= OnLocalize;
            LocalizationManager.OnLocalizeEvent += OnLocalize;
        }

        public void OnDisable()
		{
			LocalizationManager.OnLocalizeEvent -= OnLocalize;
		}

		public void OnLocalize()
		{
            onLocalize.Invoke();
        }
   }
}