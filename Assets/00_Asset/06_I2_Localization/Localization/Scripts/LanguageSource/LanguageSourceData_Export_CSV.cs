using UnityEngine;
using System.Text;

namespace I2.Loc
{
	public partial class LanguageSourceData
	{
		#region I2CSV format

		public string Export_I2CSV( string category, char separator = ',', bool specializationsAsRows=true )
		{
			StringBuilder builder = new StringBuilder ();

			//--[ Header ]----------------------------------
			builder.Append ("Key[*]Type[*]Desc");
			foreach (LanguageData langData in mLanguages)
			{
				builder.Append ("[*]");
				if (!langData.IsEnabled())
					builder.Append('$');
				builder.Append ( GoogleLanguages.GetCodedLanguage(langData.name, langData.code) );
			}
			builder.Append ("[ln]");
			
			mTerms.Sort((a, b) => string.CompareOrdinal(a.term, b.term));

			int nLanguages = (mLanguages.Count);
			bool firstLine = true;
			foreach (TermData termData in mTerms)
			{
				string term;
				
				if (string.IsNullOrEmpty(category) || (category==EmptyCategory && termData.term.IndexOfAny(CategorySeparators)<0))
					term = termData.term;
				else
					if (termData.term.StartsWith(category + @"/") && category!=termData.term)
						term = termData.term.Substring(category.Length+1);
				else
					continue;   // Term doesn't belong to this category


				if (!firstLine) builder.Append("[ln]");
                firstLine = false;

                if (!specializationsAsRows)
                {
                    AppendI2Term(builder, nLanguages, term, termData, separator, null);
                }
                else
                {
                    var allSpecializations = termData.GetAllSpecializations();
                    for (int i=0; i< allSpecializations.Count; ++i)
                    {
                        if (i!=0)
                            builder.Append("[ln]");
                        var specialization = allSpecializations[i];
                        AppendI2Term(builder, nLanguages, term, termData, separator, specialization);
                    }
                }

            }
            return builder.ToString();
		}

		static void AppendI2Term( StringBuilder builder, int nLanguages, string term, TermData termData, char separator, string forceSpecialization )
		{
            //--[ Key ] --------------
            AppendI2Text(builder, term);
            if (!string.IsNullOrEmpty(forceSpecialization) && forceSpecialization != "Any")
            {
                builder.Append("[");
                builder.Append(forceSpecialization);
                builder.Append("]");
            }
            builder.Append ("[*]");

			//--[ Type and Description ] --------------
			builder.Append (termData.termType.ToString());
			builder.Append ("[*]");
			builder.Append (termData.description);

			//--[ Languages ] --------------
			for (int i=0; i<Mathf.Min (nLanguages, termData.languages.Length); ++i)
			{
				builder.Append ("[*]");
				
				string translation = termData.languages[i];
                if (!string.IsNullOrEmpty(forceSpecialization))
                    translation = termData.GetTranslation(i, forceSpecialization);

                //bool isAutoTranslated = ((termData.Flags[i]&FlagBitMask)>0);

                /*if (translation == null)
                    translation = string.Empty;
                else
                if (translation == "")
                	translation = "-";*/
                //if (isAutoTranslated) Builder.Append("[i2auto]");
                AppendI2Text(builder, translation);
			}
		}

        static void AppendI2Text(StringBuilder builder, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (text.StartsWith("\'") || text.StartsWith("="))
                builder.Append('\'');
            builder.Append(text);
        }


        #endregion

        #region Language Cache format

        string Export_Language_to_Cache( int langIndex, bool fillTermWithFallback )
        {
            if (!mLanguages[langIndex].IsLoaded())
                return null;

            StringBuilder sb = new StringBuilder();

            for (int i=0; i<mTerms.Count; ++i)
            {
                if (i > 0)
                    sb.Append("[i2t]");
                var term = mTerms[i];
                sb.Append(term.term);
                sb.Append("=");

                string translation = term.languages[langIndex];
                if (onMissingTranslation==MissingTranslationAction.Fallback && string.IsNullOrEmpty(translation))
                {
                    if (TryGetFallbackTranslation(term, out translation, langIndex, skipDisabled: true))
                    {
                        sb.Append("[i2fb]");
                        if (fillTermWithFallback) term.languages[langIndex] = translation;
                    }
                }
                if (!string.IsNullOrEmpty(translation))
                    sb.Append(translation);
            }

            return sb.ToString();
        }

        #endregion

        #region CSV format

        public string Export_CSV( string category, char separator = ',', bool specializationsAsRows = true)
		{
			StringBuilder builder = new StringBuilder();
			
			int nLanguages = (mLanguages.Count);
			builder.AppendFormat ("Key{0}Type{0}Desc", separator);

			foreach (LanguageData langData in mLanguages)
			{
				builder.Append (separator);
				if (!langData.IsEnabled())
					builder.Append('$');
				AppendString ( builder, GoogleLanguages.GetCodedLanguage(langData.name, langData.code), separator );
			}
			builder.Append ("\n");


            mTerms.Sort((a, b) => string.CompareOrdinal(a.term, b.term));

			foreach (TermData termData in mTerms)
			{
				string term;

				if (string.IsNullOrEmpty(category) || (category==EmptyCategory && termData.term.IndexOfAny(CategorySeparators)<0))
					term = termData.term;
				else
				if (termData.term.StartsWith(category + @"/") && category!=termData.term)
					term = termData.term.Substring(category.Length+1);
				else
					continue;   // Term doesn't belong to this category

                if (specializationsAsRows)
                {
                    foreach (var specialization in termData.GetAllSpecializations())
                    {
                        AppendTerm(builder, nLanguages, term, termData, specialization, separator);
                    }
                }
                else
                {
                    AppendTerm(builder, nLanguages, term, termData, null, separator);
                }
            }
			return builder.ToString();
		}

		static void AppendTerm(StringBuilder builder, int nLanguages, string term, TermData termData, string specialization, char separator)
		{
			//--[ Key ] --------------				
			AppendString( builder, term, separator );

			if (!string.IsNullOrEmpty(specialization) && specialization!="Any")
				builder.AppendFormat( "[{0}]",specialization );
			
			//--[ Type and Description ] --------------
			builder.Append (separator);
			builder.Append (termData.termType.ToString());
			builder.Append (separator);
			AppendString(builder, termData.description, separator);
			
			//--[ Languages ] --------------
			for (int i=0; i<Mathf.Min (nLanguages, termData.languages.Length); ++i)
			{
				builder.Append (separator);

				string translation = termData.languages[i];
                if (!string.IsNullOrEmpty(specialization))
                    translation = termData.GetTranslation(i, specialization);

                //bool isAutoTranslated = ((termData.Flags[i]&FlagBitMask)>0);

                //if (string.IsNullOrEmpty(s))
                //	s = "-";

                AppendTranslation(builder, translation, separator, /*isAutoTranslated ? "[i2auto]" : */null);
			}
			builder.Append ("\n");
		}
		
		
		static void AppendString( StringBuilder builder, string text, char separator )
		{
			if (string.IsNullOrEmpty(text))
				return;
			text = text.Replace ("\\n", "\n");
			if (text.IndexOfAny((separator+"\n\"").ToCharArray())>=0)
			{
				text = text.Replace("\"", "\"\"");
				builder.AppendFormat("\"{0}\"", text);
			}
			else 
			{
				builder.Append (text);
			}
		}

		static void AppendTranslation( StringBuilder builder, string text, char separator, string tags )
		{
			if (string.IsNullOrEmpty(text))
				return;
			text = text.Replace ("\\n", "\n");
			if (text.IndexOfAny((separator+"\n\"").ToCharArray())>=0)
			{
				text = text.Replace("\"", "\"\"");
				builder.AppendFormat("\"{0}{1}\"", tags, text);
			}
			else 
			{
				builder.Append (tags);
				builder.Append (text);
			}
		}


		#endregion
	}
}