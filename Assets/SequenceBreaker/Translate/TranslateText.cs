using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Translate
{
    public class TranslateText : MonoBehaviour
    {
        //public Text targetText;

        public string targetString;


        private Text _targetText;
        private string _previousLanguage;

        private void Start()
        {
            _targetText = gameObject.GetComponent<Text>();
        }

        private void Update()
        {


            if (_previousLanguage != LocalizationManager.CurrentLanguage)
            {

                _targetText.text = Word.Get(targetString);


                // if terms and text is equal, set english font
                if (_targetText.text == targetString)
                {
                    _targetText.font = Word.Font(true);
                } else
                {
                    _targetText.font = Word.Font();
                }

                _previousLanguage = LocalizationManager.CurrentLanguage;
            }
        }


    }
}
