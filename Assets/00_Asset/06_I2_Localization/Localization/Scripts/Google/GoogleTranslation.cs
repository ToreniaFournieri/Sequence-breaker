using System.Collections.Generic;

namespace I2.Loc
{
	using TranslationDictionary = Dictionary<string, TranslationQuery>;

	public static partial class GoogleTranslation
	{
        public delegate void FnOnTranslated(string translation, string error);

        public static bool CanTranslate ()
		{
			return (LocalizationManager.Sources.Count > 0 && 
					!string.IsNullOrEmpty (LocalizationManager.GetWebServiceUrl()));
		}


        // LanguageCodeFrom can be "auto"
        // After the translation is returned from Google, it will call OnTranslationReady(TranslationResult, ErrorMsg)
        // TranslationResult will be null if translation failed
        public static void Translate( string text, string languageCodeFrom, string languageCodeTo, FnOnTranslated onTranslationReady )
		{
            LocalizationManager.InitializeIfNeeded();
            if (!GoogleTranslation.CanTranslate())
            {
                onTranslationReady(null, "WebService is not set correctly or needs to be reinstalled");
                return;
            }
            //LanguageCodeTo = GoogleLanguages.GetGoogleLanguageCode(LanguageCodeTo);

            if (languageCodeTo==languageCodeFrom)
            {
                onTranslationReady(text, null);
                return;
            }

            TranslationDictionary queries = new TranslationDictionary();


            // Unsupported language
            if (string.IsNullOrEmpty(languageCodeTo))
            {
                onTranslationReady(string.Empty, null);
                return;
            }
            CreateQueries(text, languageCodeFrom, languageCodeTo, queries);   // can split plurals and specializations into several queries

			Translate(queries, (results,error)=>
			{
					if (!string.IsNullOrEmpty(error) || results.Count==0)
					{
						onTranslationReady(null, error);
						return;
					}

					string result = RebuildTranslation( text, queries, languageCodeTo);				// gets the result from google and rebuilds the text from multiple queries if its is plurals
					onTranslationReady( result, null );
			});
		}

        // Query google for the translation and waits until google returns
        // On some Unity versions (e.g. 2017.1f1) unity doesn't handle well waiting for WWW in the main thread, so this call can fail
        // In those cases, its advisable to use the Async version  (GoogleTranslation.Translate(....))
        public static string ForceTranslate ( string text, string languageCodeFrom, string languageCodeTo )
        {
            TranslationDictionary dict = new TranslationDictionary();
            AddQuery(text, languageCodeFrom, languageCodeTo, dict);

            var job = new TranslationJobMain(dict, null);
            while (true)
            {
                var state = job.GetState();
                if (state == TranslationJob.EJobState.Running)
                    continue;

                if (state == TranslationJob.EJobState.Failed)
                    return null;

                //TranslationJob.eJobState.Succeeded
                return GetQueryResult( text, "", dict);
            }
        }

	}
}

