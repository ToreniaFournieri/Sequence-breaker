using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{
	[AddComponentMenu("I2/Localization/SetLanguage Button")]
	public class SetLanguage : MonoBehaviour 
	{
		[FormerlySerializedAs("_Language")] public string language;

#if UNITY_EDITOR
		public LanguageSource.LanguageSource mSource;
#endif
		
		void OnClick()
		{
			ApplyLanguage();
        }

		public void ApplyLanguage()
		{
			if( LocalizationManager.HasLanguage(language))
			{
				LocalizationManager.CurrentLanguage = language;
			}
		}
    }
}