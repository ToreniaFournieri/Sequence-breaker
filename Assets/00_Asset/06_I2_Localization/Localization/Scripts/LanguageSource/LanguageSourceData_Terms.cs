using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace I2.Loc
{
	public partial class LanguageSourceData
	{
        #region Language

        public void UpdateDictionary(bool force = false)
        {
            if (!force && MDictionary != null && MDictionary.Count == mTerms.Count)
                return;

            StringComparer comparer = (caseInsensitiveTerms ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            if (MDictionary.Comparer != comparer)
                MDictionary = new Dictionary<string, TermData>(comparer);
            else
                MDictionary.Clear();

            for (int i = 0, imax = mTerms.Count; i < imax; ++i)
            {
                var termData = mTerms[i];
                ValidateFullTerm(ref termData.term);

				MDictionary[termData.term]= mTerms[i];
				mTerms[i].Validate();
			}

            if (I2Utils.IsPlaying())
            {
                SaveLanguages(true);
            }
        }

        public string GetTranslation (string term, string overrideLanguage = null, string overrideSpecialization = null, bool skipDisabled = false, bool allowCategoryMistmatch = false)
		{
			string translation;
			if (TryGetTranslation(term, out translation, overrideLanguage:overrideLanguage, overrideSpecialization:overrideSpecialization, skipDisabled:skipDisabled, allowCategoryMistmatch:allowCategoryMistmatch))
				return translation;

			return string.Empty;
		}

		public bool TryGetTranslation (string term, out string translation, string overrideLanguage=null, string overrideSpecialization=null, bool skipDisabled=false, bool allowCategoryMistmatch=false)
		{
			int index = GetLanguageIndex( overrideLanguage==null ? LocalizationManager.CurrentLanguage : overrideLanguage, skipDisabled: false );

			if (index>=0 && (!skipDisabled || mLanguages[index].IsEnabled()))
			{
				TermData data = GetTermData(term, allowCategoryMistmatch:allowCategoryMistmatch);
				if (data!=null)
				{
					translation = data.GetTranslation(index, overrideSpecialization, editMode:true);

					// "---" is a code to define that the translation is meant to be empty
					if (translation == "---")
					{
						translation = string.Empty;
						return true;
					}
					else
					if (!string.IsNullOrEmpty(translation))
					{
						// has a valid translation
						return true;
					}
					else
						translation = null;
				}

				if (onMissingTranslation == MissingTranslationAction.ShowWarning)
				{
					translation = string.Format("<!-Missing Translation [{0}]-!>", term);
					return true;
				}
				else
				if (onMissingTranslation == MissingTranslationAction.Fallback && data!=null)
				{
                    return TryGetFallbackTranslation(data, out translation, index, overrideSpecialization, skipDisabled);
				}
                else
				if (onMissingTranslation == MissingTranslationAction.Empty)
                {
                    translation = string.Empty;
                    return true;
                }
                else
                if (onMissingTranslation == MissingTranslationAction.ShowTerm)
                {
                    translation = term;
                    return true;
                }

            }

            translation = null;
			return false;
		}

        bool TryGetFallbackTranslation(TermData termData, out string translation, int langIndex, string overrideSpecialization = null, bool skipDisabled = false)
        {
            // Find base Language Code
            string baseLanguage = mLanguages[langIndex].code;
            if (!string.IsNullOrEmpty(baseLanguage))
            {
                if (baseLanguage.Contains('-'))
                {
                    baseLanguage = baseLanguage.Substring(0, baseLanguage.IndexOf('-'));
                }

                // Try finding in any of the Region of the base language
                for (int i = 0; i < mLanguages.Count; ++i)
                {
                    if (i != langIndex && 
                        mLanguages[i].code.StartsWith(baseLanguage) &&
                        (!skipDisabled || mLanguages[i].IsEnabled()) )
                    {
                        translation = termData.GetTranslation(i, overrideSpecialization, editMode: true);
                        if (!string.IsNullOrEmpty(translation))
                            return true;
                    }
                }
            }


            // Otherwise, Try finding the first active language with a valid translation
            for (int i = 0; i < mLanguages.Count; ++i)
            {
                if (i!=langIndex && 
                    (!skipDisabled || mLanguages[i].IsEnabled()) && 
                    (baseLanguage==null || !mLanguages[i].code.StartsWith(baseLanguage)))
                {
                    translation = termData.GetTranslation(i, overrideSpecialization, editMode: true);
                    if (!string.IsNullOrEmpty(translation))
                        return true;
                }
            }
            translation = null;
            return false;
        }

		public TermData AddTerm( string term )
		{
			return AddTerm (term, ETermType.Text);
		}

		public TermData GetTermData( string term, bool allowCategoryMistmatch = false)
		{
			if (string.IsNullOrEmpty(term))
				return null;
			
			if (MDictionary.Count==0)// != mTerms.Count)
				UpdateDictionary();

			TermData data;
            if (MDictionary.TryGetValue(term, out data))
                return data;

			TermData d = null;
			if (allowCategoryMistmatch)
			{
				var keyPart = GetKeyFromFullTerm (term);
				foreach (var kvp in MDictionary)
					if (kvp.Value.IsTerm (keyPart, true))
					{
						if (d == null)
							d = kvp.Value;
						else
							return null;
					}
			}
			return d;
		}

		public bool ContainsTerm(string term)
		{
			return (GetTermData(term)!=null);
		}

		public List<string> GetTermsList ( string category = null )
		{
			if (MDictionary.Count != mTerms.Count)
				UpdateDictionary();
			if (string.IsNullOrEmpty( category ))
				return new List<string>( MDictionary.Keys );
			else
			{
				var terms = new List<string>();
				for (int i=0; i<mTerms.Count; ++i)
				{
					var term = mTerms[i];
					if (GetCategoryFromFullTerm( term.term ) == category)
						terms.Add( term.term );
				}
				return terms;
			}
		}

		public  TermData AddTerm( string newTerm, ETermType termType, bool saveSource = true )
		{
			ValidateFullTerm( ref newTerm );
			newTerm = newTerm.Trim ();

			if (mLanguages.Count == 0) 
				AddLanguage ("English", "en");

			// Don't duplicate Terms
			TermData data = GetTermData(newTerm);
			if (data==null) 
			{
				data = new TermData();
				data.term = newTerm;
				data.termType = termType;
				data.languages = new string[ mLanguages.Count ];
				data.flags = new byte[ mLanguages.Count ];
				mTerms.Add (data);
				MDictionary.Add ( newTerm, data);
				#if UNITY_EDITOR
				if (saveSource)
				{
                    Editor_SetDirty();
					UnityEditor.AssetDatabase.SaveAssets();
				}
				#endif
			}

			return data;
		}

		public void RemoveTerm( string term )
		{
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
				if (mTerms[i].term==term)
				{
					mTerms.RemoveAt(i);
					MDictionary.Remove(term);
					return;
				}
		}

		public static void ValidateFullTerm( ref string term )
		{
			term = term.Replace('\\', '/');
			term = term.Trim();
			if (term.StartsWith(EmptyCategory, StringComparison.Ordinal))
			{
				if (term.Length>EmptyCategory.Length && term[EmptyCategory.Length]=='/')
					term = term.Substring(EmptyCategory.Length+1);
			}
            term = I2Utils.GetValidTermName(term, true);
        }


        #endregion
    }
}