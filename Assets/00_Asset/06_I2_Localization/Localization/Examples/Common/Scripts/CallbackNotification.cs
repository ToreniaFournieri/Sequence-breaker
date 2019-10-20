using _00_Asset._06_I2_Localization.Localization.Scripts;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Examples.Common.Scripts
{

	public class CallbackNotification : MonoBehaviour 
	{
		public void OnModifyLocalization()
		{
			if (string.IsNullOrEmpty(Localize.MainTranslation))
				return;
			
			string playerColor = LocalizationManager.GetTranslation( "Color/Red" );
			
			Localize.MainTranslation = Localize.MainTranslation.Replace("{PLAYER_COLOR}", playerColor);
		}
	}
}