using _00_Asset.I2.Localization.Scripts;
using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset.I2.Localization.Examples.Common.Scripts
{

	public class CallbackNotification : MonoBehaviour 
	{
		public void OnModifyLocalization()
		{
			if (string.IsNullOrEmpty(Localize.MainTranslation))
				return;
			
			string PlayerColor = LocalizationManager.GetTranslation( "Color/Red" );
			
			Localize.MainTranslation = Localize.MainTranslation.Replace("{PLAYER_COLOR}", PlayerColor);
		}
	}
}