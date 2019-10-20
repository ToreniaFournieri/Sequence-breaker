using System.Collections.Generic;
using _00_Asset._06_I2_Localization.Localization.Scripts.Google;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Examples.Common.Scripts
{
    public class RealTimeTranslation : MonoBehaviour
    {
        string _originalText = "This is an example showing how to use the google translator to translate chat messages within the game.\nIt also supports multiline translations.",
               _translatedText = string.Empty;
        bool _isTranslating = false;

        public void OnGUI()
        {
            GUILayout.Label("Translate:");
            _originalText = GUILayout.TextArea(_originalText, GUILayout.Width(Screen.width));

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
                if (GUILayout.Button("English -> Español", GUILayout.Height(100))) StartTranslating("en", "es");
                if (GUILayout.Button("Español -> English", GUILayout.Height(100))) StartTranslating("es", "en");
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
                GUILayout.TextArea("Multiple Translation with 1 call:\n'This is an example' -> en,zh\n'Hola' -> en");
                if (GUILayout.Button("Multi Translate", GUILayout.ExpandHeight(true))) ExampleMultiTranslations_Async();
            GUILayout.EndHorizontal();


            GUILayout.TextArea(_translatedText, GUILayout.Width(Screen.width));

            GUILayout.Space(10);


            if (_isTranslating)
            {
                GUILayout.Label("Contacting Google....");
            }
        }

        public void StartTranslating(string fromCode, string toCode)
        {
            _isTranslating = true;

            // fromCode could be "auto" to autodetect the language
            GoogleTranslation.Translate(_originalText, fromCode, toCode, OnTranslationReady);

            // can also use the ForceTranslate version: (it will block the main thread until the translation is returned)
            //var translation = GoogleTranslation.ForceTranslate(OriginalText, fromCode, toCode);
            //Debug.Log(translation);
        }

        void OnTranslationReady(string translation, string errorMsg)
        {
            _isTranslating = false;

            if (errorMsg != null)
                Debug.LogError(errorMsg);
            else
                _translatedText = translation;
        }

        public void ExampleMultiTranslations_Blocking()
        {
            // This shows how to ask for many translations 
            var dict = new System.Collections.Generic.Dictionary<string, TranslationQuery>();
            GoogleTranslation.AddQuery("This is an example", "en", "es", dict);
            GoogleTranslation.AddQuery("This is an example", "auto", "zh", dict);
            GoogleTranslation.AddQuery("Hola", "es", "en", dict);

            if (!GoogleTranslation.ForceTranslate(dict))
                return;

            Debug.Log(GoogleTranslation.GetQueryResult("This is an example", "en", dict));
            Debug.Log(GoogleTranslation.GetQueryResult("This is an example", "zh", dict));
            Debug.Log(GoogleTranslation.GetQueryResult("This is an example", "", dict));  // This returns ANY translation of that text (in this case, the first one 'en')
            Debug.Log(dict["Hola"].Results[0]); // example of getting the translation directly from the Results
        }

        public void ExampleMultiTranslations_Async()
        {
            _isTranslating = true;

            // This shows how to ask for many translations 
            var dict = new Dictionary<string, TranslationQuery>();
            GoogleTranslation.AddQuery("This is an example", "en", "es", dict);
            GoogleTranslation.AddQuery("This is an example", "auto", "zh", dict);
            GoogleTranslation.AddQuery("Hola", "es", "en", dict);

            GoogleTranslation.Translate(dict, OnMultitranslationReady);
        }

        void OnMultitranslationReady(Dictionary<string, TranslationQuery> dict, string errorMsg)
        {
            if (!string.IsNullOrEmpty(errorMsg))
            {
                Debug.LogError(errorMsg);
                return;
            }

            _isTranslating = false;
            _translatedText = "";

            _translatedText += GoogleTranslation.GetQueryResult("This is an example", "es", dict) + "\n";
            _translatedText += GoogleTranslation.GetQueryResult("This is an example", "zh", dict) + "\n";
            _translatedText += GoogleTranslation.GetQueryResult("This is an example", "", dict) + "\n";    // This returns ANY translation of that text (in this case, the first one 'en')
            _translatedText += dict["Hola"].Results[0];                                                    // example of getting the translation directly from the Results
        }

        #region This methods are used in the publisher's Unity Tests

        public bool IsWaitingForTranslation()
        {
            return _isTranslating;
        }

        public string GetTranslatedText()
        {
            return _translatedText;
        }

        public void SetOriginalText( string text )
        {
            _originalText = text;
        }

        #endregion

    }
}