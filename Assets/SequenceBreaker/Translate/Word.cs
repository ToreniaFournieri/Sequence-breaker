using _00_Asset.I2.Localization.Scripts.Manager;

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
            string words = LocalizationManager.GetTranslation(term);

            if (words == null) { words = term; }
            //example) "plural X damages.[i2p_Zero] No damage.[i2p_One] 1 damage."
            int i2p_ZeroLength = words.IndexOf("[i2p_Zero]", System.StringComparison.Ordinal);
            int i2p_OneLength = words.IndexOf("[i2p_One]", System.StringComparison.Ordinal);

            if (value == "0")
            {
                if (i2p_OneLength == -1)
                {
                    if (i2p_ZeroLength == -1) { /* no need to trim. */  }
                    else { words = words.Substring(0, i2p_ZeroLength); }
                }
                else { words = words.Substring(i2p_OneLength + 9); }
            }
            else if (value == "1")
            {
                if (i2p_OneLength == -1)
                {
                    if (i2p_ZeroLength == -1) { /* no need to trim. */  }
                    else { words = words.Substring(0, i2p_ZeroLength); }
                }
                else { words = words.Substring(0, words.Length - (i2p_OneLength + 9)); }
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
            if (words.Contains("{[VALUE]}")) { words = words.Replace("{[VALUE]}", value); }

            return words;
        }


    }
}
