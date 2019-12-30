using _00_Asset.I2.Localization.Scripts.Manager;

namespace SequenceBreaker.Translate
{
    public static class Word
    {

        public static string Get(string term)
        {
            string _term = null;
            _term = LocalizationManager.GetTranslation(term);

            if (_term == null) { _term = term; }

            return _term;

        }


    }
}
