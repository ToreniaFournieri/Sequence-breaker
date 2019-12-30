using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Translate
{
    public class SelectLanguage : MonoBehaviour
    {

        private Dropdown _dropdown;


        private void Start()
        {
            _dropdown = gameObject.GetComponent<Dropdown>();

            int value = 0;
            foreach (var optionData in _dropdown.options)
            {

                if(optionData.text.Contains(LocalizationManager.CurrentLanguage))
                {
                    _dropdown.value = value;
                }
                value++;

            }
        }

        //void OnClick()
        //{
        //}

        public void Selected()
        {
            if (LocalizationManager.HasLanguage(_dropdown.options[_dropdown.value].text))
            {
                LocalizationManager.CurrentLanguage = _dropdown.options[_dropdown.value].text;
            }
        }

    }
}
