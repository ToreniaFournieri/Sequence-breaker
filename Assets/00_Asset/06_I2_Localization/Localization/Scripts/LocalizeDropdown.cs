using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace I2.Loc
{
	#if !UNITY_5_0 && !UNITY_5_1
    [AddComponentMenu("I2/Localization/Localize Dropdown")]
	public class LocalizeDropdown : MonoBehaviour
	{
        [FormerlySerializedAs("_Terms")] public List<string> terms = new List<string>();
		
		public void Start()
		{
			LocalizationManager.OnLocalizeEvent += OnLocalize;
            OnLocalize();
		}
		
		public void OnDestroy()
		{
			LocalizationManager.OnLocalizeEvent -= OnLocalize;
		}

        void OnEnable()
        {
            if (terms.Count == 0)
                FillValues();
            OnLocalize ();
        }
		
		public void OnLocalize()
		{
            if (!enabled || gameObject==null || !gameObject.activeInHierarchy)
                return;

            if (string.IsNullOrEmpty(LocalizationManager.CurrentLanguage))
                return;

            UpdateLocalization();
        }

        void FillValues()
        {
            var dropdown = GetComponent<Dropdown>();
            if (dropdown == null && I2Utils.IsPlaying())
            {
                #if TextMeshPro
                    FillValuesTmPro();
                #endif
                return;
            }

            foreach (var term in dropdown.options)
            {
                terms.Add(term.text);
            }
        }

        public void UpdateLocalization()
		{
			var dropdown = GetComponent<Dropdown>();
            if (dropdown == null)
            {
                #if TextMeshPro
                    UpdateLocalizationTmPro();
                #endif
                return;
            }
			
			dropdown.options.Clear();
			foreach (var term in terms)
			{
                var translation = LocalizationManager.GetTranslation(term);
				dropdown.options.Add( new Dropdown.OptionData( translation ) );
			}
            dropdown.RefreshShownValue();
		}

        #if TextMeshPro
        public void UpdateLocalizationTmPro()
        {
            var dropdown = GetComponent<TMPro.TMP_Dropdown>();
            if (dropdown == null)
                return;

            dropdown.options.Clear();
            foreach (var term in terms)
            {
                var translation = LocalizationManager.GetTranslation(term);
                dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(translation));
            }
            dropdown.RefreshShownValue();
        }

        void FillValuesTmPro()
        {
            var dropdown = GetComponent<TMPro.TMP_Dropdown>();
            if (dropdown == null)
                return;

            foreach (var term in dropdown.options)
            {
                terms.Add(term.text);
            }
        }
#endif

    }
#endif
}