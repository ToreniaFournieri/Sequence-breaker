using _00_Asset.I2.Localization.Scripts.Manager;
using UnityEngine;

namespace SequenceBreaker.Translate
{
    public static class Word
    {


        //Translation without parameter
        public static string Get(string term)
        {
            string words = LocalizationManager.GetTranslation(term);
            if (words == null) { words = term; }
            return words;
        }

        //Translation with parameter
        // ex) "LocalizationManager.GetTranslation(term)" will return value like:"plural {[X]} damages.[i2p_Zero] No damage.[i2p_One] 1 damage."
        // So I need to trim to get an appropriate term.
        public static string Get(string term, string valueX)
        {
            string words = LocalizationManager.GetTranslation(term);

            if (words == null) { words = term; }
            int i2p_ZeroLength = words.IndexOf("[i2p_Zero]", System.StringComparison.Ordinal);
            int i2p_OneLength = words.IndexOf("[i2p_One]", System.StringComparison.Ordinal);

            // remove ordinal numeral.
            string removedOrdinalX = valueX;
            if (valueX.Contains(Get("ORDINAL-1st"))) { removedOrdinalX = valueX.Replace(Get("ORDINAL-1st"), null); }
            if (valueX.Contains(Get("ORDINAL-2nd"))) { removedOrdinalX = valueX.Replace(Get("ORDINAL-2nd"), null); }
            if (valueX.Contains(Get("ORDINAL-3rd"))) { removedOrdinalX = valueX.Replace(Get("ORDINAL-3rd"), null); }
            if (valueX.Contains(Get("ORDINAL-Xth"))) { removedOrdinalX = valueX.Replace(Get("ORDINAL-Xth"), null); }

            if (removedOrdinalX == "0")
            {
                if (i2p_ZeroLength == -1)
                {
                    if (i2p_OneLength == -1) { /* no need to trim. */  }
                    else { words = words.Substring(0, i2p_OneLength); }
                }
                else { words = words.Substring(i2p_ZeroLength + 9); }
            }
            else if (removedOrdinalX == "1")
            {
                if (i2p_OneLength == -1)
                {
                    if (i2p_ZeroLength == -1) { /* no need to trim. */  }
                    else { words = words.Substring(0, i2p_ZeroLength); }
                }
                else { words = words.Substring(i2p_OneLength + 9); }
            }
            else
            {
                if (i2p_ZeroLength == -1)
                {
                    if (i2p_OneLength == -1) { /* no need to trim */  }
                    else { words = words.Substring(0, i2p_OneLength); }
                }
                else { words = words.Substring(0, i2p_ZeroLength); }
            }

            if (words.Contains("{[X]}")) { words = words.Replace("{[X]}", valueX); }

            return words;
        }


        public static Font Font()
        {
            return LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");
        }

        public static Font Font(bool isAlphabet)
        {
            if (isAlphabet)
            {
                return LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT-Alphabet");
            }
            else
            {
                return Font();
            }
        }

        public static string Get(string term, string ordinalNumeral, bool isOrdinals)
        {
            string valueX = ordinalNumeral;
            if (isOrdinals)
            {
                if (bool.Parse(Get("ORDINAL")))
                {

                    int ordinalInt = 0;
                    try
                    {
                        ordinalInt = int.Parse(ordinalNumeral);
                    }
                    catch
                    {
                        Debug.LogError("Word.Get invailed ordinal Numeral value. term: " + term + " ordinal Numeral: " + ordinalNumeral);
                        return Get(term, ordinalNumeral);
                    }

                    if ((ordinalInt % 100) == 11 || (ordinalInt % 100) == 12 || (ordinalInt % 100) == 13)
                    {
                        valueX = ordinalNumeral + "th";
                    }
                    else
                        switch (ordinalInt % 10)
                        {
                            case 1: valueX = ordinalNumeral + Get("ORDINAL-1st"); break;
                            case 2: valueX = ordinalNumeral + Get("ORDINAL-2nd"); break;
                            case 3: valueX = ordinalNumeral + Get("ORDINAL-3rd"); break;
                            default: valueX = ordinalNumeral + Get("ORDINAL-4th"); break;
                        }
                }
            }
            return Get(term, valueX);
        }


        

    }


}