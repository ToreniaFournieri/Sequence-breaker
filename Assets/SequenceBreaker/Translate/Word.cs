using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;

namespace SequenceBreaker.Translate
{
    public static class Word
    {

        public static string Get(string term)
        {
            string words = null;
            words = LocalizationManager.GetTranslation(term);

            if (words == null) { words = term; }

            return words;

        }

        public static string GetWithParam(string term, string param1)
        {
            string words = null;
            words = LocalizationManager.GetTranslation(term);

            if (words == null) { words = term; }

            //if (words.Contains("は"))
            //{
            //    Debug.Log("は exist in :" + words + "param1 :" + param1);
            //    words.Replace("は", param1);
            //}

            //Debug.Log(words);

            if (words.Contains("{[VALUE]}"))
            {
                Debug.Log("{[VALUE]} exist in :" + words + "param1 :" + param1);
               　words = words.Replace("{[VALUE]}", param1);
            }

            return words;
        }


    }
}
