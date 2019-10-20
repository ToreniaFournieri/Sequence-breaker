using UnityEngine;

namespace I2.Loc
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