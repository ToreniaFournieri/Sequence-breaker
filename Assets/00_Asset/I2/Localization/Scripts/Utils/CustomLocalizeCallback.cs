using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.Events;

namespace _00_Asset.I2.Localization.Scripts.Utils
{
    [AddComponentMenu("I2/Localization/I2 Localize Callback")]
	public class CustomLocalizeCallback : MonoBehaviour
	{
        public UnityEvent _OnLocalize = new UnityEvent();
		
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
            _OnLocalize.Invoke();
        }
   }
}