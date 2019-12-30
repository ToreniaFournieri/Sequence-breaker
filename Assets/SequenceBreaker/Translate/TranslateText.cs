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

<<<<<<< HEAD
            if (_previousLanguage != LocalizationManager.CurrentLanguage)
            {

                Debug.Log(Word.Get(targetString));

                _targetText.text = LocalizationManager.GetTranslation(targetString);
=======
            _targetText.text = LocalizationManager.GetTranslation(targetString);
>>>>>>> parent of cb7c369c4... Word translate

                _previousLanguage = LocalizationManager.CurrentLanguage;
            }
        }


    }
}
