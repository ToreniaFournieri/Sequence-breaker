using UnityEngine;
using System;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LanguageSourceData
	{
		public string Import_CSV( string category, string csVstring, ESpreadsheetUpdateMode updateMode = ESpreadsheetUpdateMode.Replace, char separator = ',' )
		{
			List<string[]> csv = LocalizationReader.ReadCsv (csVstring, separator);
			return Import_CSV( category, csv, updateMode );
		}

		public string Import_I2CSV( string category, string i2CsVstring, ESpreadsheetUpdateMode updateMode = ESpreadsheetUpdateMode.Replace )
		{
			List<string[]> csv = LocalizationReader.ReadI2Csv (i2CsVstring);
			return Import_CSV( category, csv, updateMode );
		}

		public string Import_CSV( string category, List<string[]> csv, ESpreadsheetUpdateMode updateMode = ESpreadsheetUpdateMode.Replace )
		{
			string[] tokens = csv[0];

			int languagesStartIdx = 1;
			int typeColumnIdx = -1;
			int descColumnIdx = -1;

			var validColumnNameKey  = new string[]{ "Key" };
			var validColumnNameType = new string[]{ "Type" };
			var validColumnNameDesc = new string[]{ "Desc", "Description" };

			if (tokens.Length>1 && ArrayContains(tokens[0], validColumnNameKey))
			{
				if (updateMode == ESpreadsheetUpdateMode.Replace)
					ClearAllData();

				if (tokens.Length>2)
				{
					if (ArrayContains(tokens[1], validColumnNameType)) 
					{
						typeColumnIdx = 1;
						languagesStartIdx = 2;
					}
					if (ArrayContains(tokens[1], validColumnNameDesc)) 
					{
						descColumnIdx = 1;
						languagesStartIdx = 2;
					}

				}
				if (tokens.Length>3)
				{
					if (ArrayContains(tokens[2], validColumnNameType)) 
					{
						typeColumnIdx = 2;
						languagesStartIdx = 3;
					}
					if (ArrayContains(tokens[2], validColumnNameDesc)) 
					{
						descColumnIdx = 2;
						languagesStartIdx = 3;
					}
				}
			}
			else
				return "Bad Spreadsheet Format.\nFirst columns should be 'Key', 'Type' and 'Desc'";

			int nLanguages = Mathf.Max (tokens.Length-languagesStartIdx, 0);
			int[] lanIndices = new int[nLanguages];
			for (int i=0; i<nLanguages; ++i)
			{
				if (string.IsNullOrEmpty(tokens[i+languagesStartIdx]))
				{
					lanIndices [i] = -1;
					continue;
				}

				string langToken = tokens[i + languagesStartIdx];

				string lanName, lanCode;
				bool isLangEnabled = true;
				if (langToken.StartsWith("$"))
				{
					isLangEnabled = false;
					langToken = langToken.Substring(1);
				}
				GoogleLanguages.UnPackCodeFromLanguageName(langToken, out lanName, out lanCode);

				int lanIdx = -1;
				if (!string.IsNullOrEmpty(lanCode))
					lanIdx = GetLanguageIndexFromCode(lanCode);
				else
					lanIdx = GetLanguageIndex(lanName, skipDisabled:false);

				if (lanIdx < 0)
				{
					LanguageData lanData = new LanguageData();
					lanData.name = lanName;
					lanData.code = lanCode;
					lanData.flags = (byte)(0 | (isLangEnabled?0:(int)ELanguageDataFlags.Disabled));
					mLanguages.Add (lanData);
					lanIdx = mLanguages.Count-1;
				}
				lanIndices[i] = lanIdx;
			}

			//--[ Update the Languages array in the existing terms]-----
			nLanguages = mLanguages.Count;
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
			{
				TermData termData = mTerms[i];
				if (termData.languages.Length < nLanguages)
				{
					Array.Resize( ref termData.languages, nLanguages );
					Array.Resize( ref termData.flags, nLanguages );
				}
			}

            //--[ Keys ]--------------

            for (int i = 1, imax = csv.Count; i < imax; ++i)
            {
                tokens = csv[i];
                string sKey = string.IsNullOrEmpty(category) ? tokens[0] : string.Concat(category, "/", tokens[0]);

                string specialization = null;
                if (sKey.EndsWith("]"))
                {
                    int idx = sKey.LastIndexOf('[');
                    if (idx>0)
                    {
                        specialization = sKey.Substring(idx + 1, sKey.Length - idx - 2);
                        if (specialization == "touch") specialization = "Touch";
                        sKey = sKey.Remove(idx);
                    }
                }
				ValidateFullTerm(ref sKey);
				if (string.IsNullOrEmpty(sKey))
					continue;

				TermData termData = GetTermData(sKey);

				// Check to see if its a new term
				if (termData==null)
				{
					termData = new TermData();
					termData.term = sKey;

					termData.languages = new string[ mLanguages.Count ];
					termData.flags = new byte[ mLanguages.Count ];
					for (int j=0; j<mLanguages.Count; ++j) 
						termData.languages[j] = string.Empty;

					mTerms.Add (termData);
					MDictionary.Add (sKey, termData);
				}
				else
				// This term already exist
				if (updateMode==ESpreadsheetUpdateMode.AddNewTerms)
					continue;

				if (typeColumnIdx>0)
					termData.termType = GetTermType(tokens[typeColumnIdx]);

				if (descColumnIdx>0)
					termData.description = tokens[descColumnIdx];

                for (int j = 0; j < lanIndices.Length && j < tokens.Length - languagesStartIdx; ++j)
                    if (!string.IsNullOrEmpty(tokens[j + languagesStartIdx]))   // Only change the translation if there is a new value
                    {
                        var lanIdx = lanIndices[j];
                        if (lanIdx < 0)
                            continue;
                        var value = tokens[j + languagesStartIdx];

                        if (value == "-")
                            value = string.Empty;
                        else
                        if (value == "")
                            value = null;

                        termData.SetTranslation(lanIdx, value, specialization);
                    }
            }
            if (Application.isPlaying)
            {
                SaveLanguages(HasUnloadedLanguages());
            }
            return string.Empty;
		}

		bool ArrayContains( string mainText, params string[] texts )
		{
			for (int i=0, imax=texts.Length; i<imax; ++i)
				if (mainText.IndexOf(texts[i], StringComparison.OrdinalIgnoreCase)>=0)
					return true;
			return false;
		}

		public static ETermType GetTermType( string type )
		{
			for (int i=0, imax=(int)ETermType.Object; i<=imax; ++i)
				if (string.Equals( ((ETermType)i).ToString(), type, StringComparison.OrdinalIgnoreCase))
					return (ETermType)i;
			
			return ETermType.Text;
		}

        #region Language Cache format

        void Import_Language_from_Cache(int langIndex, string langData, bool useFallback, bool onlyCurrentSpecialization)
        {
            int index = 0;
            while (index < langData.Length)
            {
                int nextIndex = langData.IndexOf("[i2t]", index);
                if (nextIndex < 0) nextIndex = langData.Length;

                // check for errors
                int termNameEnd = langData.IndexOf("=", index);
                if (termNameEnd >= nextIndex)
                    return;

                string termName = langData.Substring(index, termNameEnd - index);
                index = termNameEnd+1;

                var termData = GetTermData(termName, false);
                if (termData != null)
                {
                    string translation = null;

                    if (index != nextIndex)
                    {
                        translation = langData.Substring(index, nextIndex - index);
                        if (translation.StartsWith("[i2fb]"))
                        {
                            translation = (useFallback) ? translation.Substring(6) : null;
                        }
                        if (onlyCurrentSpecialization && translation != null)
                        {
                            translation = SpecializationManager.GetSpecializedText(translation, null);
                        }
                    }
                    termData.languages[langIndex] = translation;
                }
                index = nextIndex + 5;
            }
        }

        #endregion
    }
}