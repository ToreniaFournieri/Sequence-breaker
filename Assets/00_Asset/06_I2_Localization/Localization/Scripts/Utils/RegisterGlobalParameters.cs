using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{

    public class RegisterGlobalParameters : MonoBehaviour, ILocalizationParamsManager 
	{
		public virtual void OnEnable()
		{
            if (!LocalizationManager.ParamManagers.Contains(this))
            {
                LocalizationManager.ParamManagers.Add(this);
                LocalizationManager.LocalizeAll(true);
            }
		}

		public virtual void OnDisable()
        {
            LocalizationManager.ParamManagers.Remove(this);
        }

		public virtual string GetParameterValue( string paramName )
        {
            return null;
        }

	}
}