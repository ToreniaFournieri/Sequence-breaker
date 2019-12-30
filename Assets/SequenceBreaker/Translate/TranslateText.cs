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

                
                Debug.Log("font " + LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT").name);
                _targetText.font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");

                _previousLanguage = LocalizationManager.CurrentLanguage;
            }
        }


    }
}
