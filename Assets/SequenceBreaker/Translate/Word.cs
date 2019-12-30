using _00_Asset.I2.Localization.Scripts.Manager;

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

            if (valueX == "0")
            {
                if (i2p_ZeroLength == -1)
                {
                    if (i2p_OneLength == -1) { /* no need to trim. */  }
                    else { words = words.Substring(0, i2p_OneLength); }
                }
                else { words = words.Substring(i2p_ZeroLength + 9); }
            }
            else if (valueX == "1")
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
    }
}
