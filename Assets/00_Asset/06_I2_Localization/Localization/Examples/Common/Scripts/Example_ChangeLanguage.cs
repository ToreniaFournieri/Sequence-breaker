using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Examples.Common.Scripts
{
	public class ExampleChangeLanguage : MonoBehaviour 
	{
		public void SetLanguage_English()
		{
			SetLanguage("English");
		}

		public void SetLanguage_French()
		{
			SetLanguage("French");
		}

		public void SetLanguage_Spanish()
		{
			SetLanguage("Spanish");
		}


		public void SetLanguage( string langName )
		{
			if( LocalizationManager.HasLanguage(langName))
			{
				LocalizationManager.CurrentLanguage = langName;
			}
		}

	}
}