using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset.I2.Localization.Scripts.Utils
{
	[AddComponentMenu("I2/Localization/SetLanguage Button")]
	public class SetLanguage : MonoBehaviour 
	{
		public string _Language;

#if UNITY_EDITOR
		public LanguageSource.LanguageSource mSource;
#endif
		
		void OnClick()
		{
			ApplyLanguage();
        }

		public void ApplyLanguage()
		{
			if( LocalizationManager.HasLanguage(_Language))
			{
				LocalizationManager.CurrentLanguage = _Language;
			}
		}
    }
}