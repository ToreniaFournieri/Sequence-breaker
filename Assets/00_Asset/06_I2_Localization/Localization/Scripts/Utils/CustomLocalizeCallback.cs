using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace I2.Loc
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