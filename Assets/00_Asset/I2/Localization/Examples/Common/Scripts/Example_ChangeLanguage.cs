using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset.I2.Localization.Examples.Common.Scripts
{
	public class Example_ChangeLanguage : MonoBehaviour 
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


		public void SetLanguage( string LangName )
		{
			if( LocalizationManager.HasLanguage(LangName))
			{
				LocalizationManager.CurrentLanguage = LangName;
			}
		}

	}
}