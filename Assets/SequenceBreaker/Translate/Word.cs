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

        public static string GetWithParam(string term, string value)
        {
            string words = null;
            words = LocalizationManager.GetTranslation(term);

            if (words == null) { words = term; }


            //example) "plural X damages.[i2p_Zero] No damage.[i2p_One] 1 damage."
            //Plural 0
            int i2p_ZeroIndex = words.IndexOf("[i2p_Zero]", System.StringComparison.Ordinal);
            int i2p_OneIndex = words.IndexOf("[i2p_One]", System.StringComparison.Ordinal);

            Debug.Log("[i2p_Zero]: " + i2p_ZeroIndex + " i2p_OneIndex: " + i2p_OneIndex + " word: " + words);

            if (value == "0")
            {
                if (i2p_OneIndex == -1)
                {
                    //No special word exist, "plural X damages." should be selected.
                    if (i2p_ZeroIndex == -1)
                    {
                        // no need to trim.
                    }
                    else
                    {
                        words = words.Substring(0, i2p_ZeroIndex);
                    }
                }
                else
                {
                    // One index exist. use them.
                    words = words.Substring(i2p_OneIndex + 9);
                }


            }
            else if (value == "1")
            {
                if (i2p_OneIndex == -1)
                {
                    if (i2p_ZeroIndex == -1)
                    {
                        // no need to trim.
                    }
                    else
                    {
                        words = words.Substring(0, i2p_ZeroIndex);
                    }
                }
                else
                {
                    words = words.Substring(0, words.Length - (i2p_OneIndex + 9));
                }
            }
            else
            {
                if (i2p_ZeroIndex == -1)
                {
                    if (i2p_OneIndex == -1)
                    {
                        // no need to trim
                    }
                    else
                    {
                        words = words.Substring(0, i2p_OneIndex);
                    }
                }
                else
                {
                    words = words.Substring(0, i2p_ZeroIndex);
                }
            }



            if (words.Contains("{[VALUE]}"))
            {
                //Debug.Log("{[VALUE]} exist in :" + words + "param1 :" + value);
                words = words.Replace("{[VALUE]}", value);
            }

            return words;
        }


    }
}
