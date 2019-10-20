using UnityEngine;

namespace I2.Loc
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