using System;
using System.Collections.Generic;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{
    public interface ILocalizationParamsManager
    {
        string GetParameterValue( string param );
    }

    public class LocalizationParamsManager : MonoBehaviour, ILocalizationParamsManager
	{
        [Serializable]
        public struct ParamValue
        {
            [FormerlySerializedAs("Name")] public string name;
            [FormerlySerializedAs("Value")] public string value;
        }

        [SerializeField]
        public List<ParamValue> _Params = new List<ParamValue>();

        [FormerlySerializedAs("_IsGlobalManager")] public bool isGlobalManager;
        
        public string GetParameterValue( string paramName )
        {
            if (_Params != null)
            {
                for (int i = 0, imax = _Params.Count; i < imax; ++i)
                    if (_Params[i].name == paramName)
                        return _Params[i].value;
            }
            return null; // not found
        }

        public void SetParameterValue( string paramName, string paramValue, bool localize = true )
        {
            bool setted = false;
            for (int i = 0, imax = _Params.Count; i < imax; ++i)
                if (_Params[i].name == paramName)
                {
                    var temp = _Params[i];
                    temp.value = paramValue;
                    _Params[i] = temp;
                    setted = true;
                    break;
                }
            if (!setted)
                _Params.Add(new ParamValue(){ name = paramName, value = paramValue });
        
			if (localize)
				OnLocalize();
		}
		
		public void OnLocalize()
		{
            var loc = GetComponent<Localize>();
            if (loc != null)
                loc.OnLocalize(true);
        }

        public virtual void OnEnable()
        {
            if (isGlobalManager)
                DoAutoRegister();
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //public void AutoStart()
        //{
        //    if (_AutoRegister)
        //        DoAutoRegister();
        //}

        public void DoAutoRegister()
        {
            if (!LocalizationManager.ParamManagers.Contains(this))
            {
                LocalizationManager.ParamManagers.Add(this);
                LocalizationManager.LocalizeAll(true);
            }
        }

        public void OnDisable()
        {
            LocalizationManager.ParamManagers.Remove(this);
        }
    }
}