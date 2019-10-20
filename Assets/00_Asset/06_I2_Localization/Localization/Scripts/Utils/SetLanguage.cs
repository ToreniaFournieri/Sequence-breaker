using UnityEngine;
using UnityEngine.Serialization;

namespace I2.Loc
{
	[AddComponentMenu("I2/Localization/SetLanguage Button")]
	public class SetLanguage : MonoBehaviour 
	{
		[FormerlySerializedAs("_Language")] public string language;

#if UNITY_EDITOR
		public LanguageSource mSource;
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