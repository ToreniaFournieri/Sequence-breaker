using System;
using System.Linq;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LanguageSourceData
	{
		#region Language

		public int GetLanguageIndex( string language, bool allowDiscartingRegion = true, bool skipDisabled = true)
		{
			// First look for an exact match
			for (int i=0, imax=mLanguages.Count; i<imax; ++i)
				if ((!skipDisabled || mLanguages[i].IsEnabled()) && string.Compare(mLanguages[i].name, language, StringComparison.OrdinalIgnoreCase)==0)
					return i;

			// Then allow matching "English (Canada)" to "english"
			if (allowDiscartingRegion)
			{
				int mostSimilar = -1;
				int bestSimilitud = 0;
				for (int i=0, imax=mLanguages.Count; i<imax; ++i)
					if (!skipDisabled || mLanguages[i].IsEnabled())
					{
						int commonWords = GetCommonWordInLanguageNames(mLanguages[i].name, language);
						if (commonWords>bestSimilitud)
						{
							bestSimilitud = commonWords;
							mostSimilar = i;
						}
						//if (AreTheSameLanguage(mLanguages[i].Name, language))
						//	return i;
					}
				if (mostSimilar>=0)
					return mostSimilar;
			}
			return -1;
		}

        public LanguageData GetLanguageData(string language, bool allowDiscartingRegion = true)
        {
            int idx = GetLanguageIndex(language, allowDiscartingRegion, false);
            return (idx < 0) ? null : mLanguages[idx];
        }

        // TODO: Fix IsCurrentLanguage when current=English  and there are two variants in the source (English Canada, English US)
        public bool IsCurrentLanguage( int languageIndex )
        {
            return LocalizationManager.CurrentLanguage == mLanguages[languageIndex].name;
        }

        public int GetLanguageIndexFromCode( string code, bool exactMatch=true, bool ignoreDisabled = false)
		{
            for (int i = 0, imax = mLanguages.Count; i < imax; ++i)
            {
                if (ignoreDisabled && !mLanguages[i].IsEnabled())
                    continue;

                if (string.Compare(mLanguages[i].code, code, StringComparison.OrdinalIgnoreCase) == 0)
                    return i;
            }

			if (!exactMatch)
			{
                // Find any match without using the Regions
                for (int i = 0, imax = mLanguages.Count; i < imax; ++i)
                {
                    if (ignoreDisabled && !mLanguages[i].IsEnabled())
                        continue;

                    if (string.Compare(mLanguages[i].code, 0, code, 0, 2, StringComparison.OrdinalIgnoreCase) == 0)
                        return i;
                }
			}

			return -1;
		}

		public static int GetCommonWordInLanguageNames(string language1, string language2)
		{
			if (string.IsNullOrEmpty (language1) || string.IsNullOrEmpty (language2))
					return 0;
			var separators = "( )-/\\".ToCharArray();
			string[] words1 = language1.ToLower().Split(separators);
			string[] words2 = language2.ToLower().Split(separators);

			int similitud = 0;
			foreach (var word in words1)
				if (!string.IsNullOrEmpty(word) && words2.Contains(word))
					similitud++;

			foreach (var word in words2)
				if (!string.IsNullOrEmpty(word) && words1.Contains(word))
					similitud++;

			return similitud;
		}

		public static bool AreTheSameLanguage(string language1, string language2)
		{
			language1 = GetLanguageWithoutRegion(language1);
			language2 = GetLanguageWithoutRegion(language2);
			return (string.Compare(language1, language2, StringComparison.OrdinalIgnoreCase)==0);
		}

		public static string GetLanguageWithoutRegion(string language)
		{
			int index = language.IndexOfAny("(/\\[,{".ToCharArray());
			if (index<0)
				return language;
			else
				return language.Substring(0, index).Trim();
		}

        public void AddLanguage(string languageName)
        {
            AddLanguage(languageName, GoogleLanguages.GetLanguageCode(languageName));
        }

        public void AddLanguage( string languageName, string languageCode )
		{
			if (GetLanguageIndex(languageName, false)>=0)
				return;

			LanguageData lang = new LanguageData();
				lang.name = languageName;
				lang.code = languageCode;
			mLanguages.Add (lang);

			int newSize = mLanguages.Count;
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
			{
				Array.Resize(ref mTerms[i].languages, newSize);
				Array.Resize(ref mTerms[i].flags, newSize);
			}
            Editor_SetDirty();
        }

		public void RemoveLanguage( string languageName )
		{
			int langIndex = GetLanguageIndex(languageName, false, false);
			if (langIndex<0)
				return;

			int nLanguages = mLanguages.Count;
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
			{
				for (int j=langIndex+1; j<nLanguages; ++j)
				{
					mTerms[i].languages[j-1] = mTerms[i].languages[j];
					mTerms[i].flags[j-1] = mTerms[i].flags[j];
				}
				Array.Resize(ref mTerms[i].languages, nLanguages-1);
				Array.Resize(ref mTerms[i].flags, nLanguages-1);
			}
			mLanguages.RemoveAt(langIndex);
            Editor_SetDirty();

        }

		public List<string> GetLanguages( bool skipDisabled = true)
		{
			List<string> languages = new List<string>();
			for (int j = 0, jmax = mLanguages.Count; j < jmax; ++j)
			{
				if (!skipDisabled || mLanguages[j].IsEnabled())
					languages.Add(mLanguages[j].name);
			}
			return languages;
		}

		public List<string> GetLanguagesCode(bool allowRegions = true, bool skipDisabled = true)
		{
			List<string> languages = new List<string>();
			for (int j = 0, jmax = mLanguages.Count; j < jmax; ++j)
			{
				if (skipDisabled && !mLanguages[j].IsEnabled())
					continue;

				var code = mLanguages[j].code;

				if (!allowRegions && code != null && code.Length > 2)
					code = code.Substring(0, 2);

				if (!string.IsNullOrEmpty(code) && !languages.Contains(code))
					languages.Add(code);
			}
			return languages;
		}

		public bool IsLanguageEnabled(string language)
		{
			int idx = GetLanguageIndex(language, false);
			return idx >= 0 && mLanguages[idx].IsEnabled();
		}

        public void EnableLanguage(string language, bool bEnabled)
        {
            int idx = GetLanguageIndex(language, false, false);
            if (idx >= 0)
                mLanguages[idx].SetEnabled(bEnabled);
        }

        #endregion

        #region Save/Load Language

        public bool AllowUnloadingLanguages()
        {
            #if UNITY_EDITOR
                return allowUnloadingLanguages==EAllowUnloadLanguages.EditorAndDevice;
            #else
                return _AllowUnloadingLanguages!=eAllowUnloadLanguages.Never;
            #endif
        }

        string GetSavedLanguageFileName(int languageIndex)
        {
            if (languageIndex < 0)
                return null;

            return "LangSource_" + GetSourcePlayerPrefName() + "_" + mLanguages[languageIndex].name + ".loc";
        }
        public void LoadLanguage( int languageIndex, bool unloadOtherLanguages, bool useFallback, bool onlyCurrentSpecialization, bool forceLoad )
		{
            if (!AllowUnloadingLanguages())
                return;

            // Some consoles don't allow IO access
            if (!PersistentStorage.CanAccessFiles())
                return;

            if (languageIndex >= 0 && (forceLoad || !mLanguages[languageIndex].IsLoaded()))
            {
                var tempPath = GetSavedLanguageFileName(languageIndex);
                var langData = PersistentStorage.LoadFile(PersistentStorage.EFileType.Temporal, tempPath, false);

                if (!string.IsNullOrEmpty(langData))
                {
                    Import_Language_from_Cache(languageIndex, langData, useFallback, onlyCurrentSpecialization);
                    mLanguages[languageIndex].SetLoaded(true);
                }
            }
            if (unloadOtherLanguages && I2Utils.IsPlaying())
            {
                for (int lan = 0; lan < mLanguages.Count; ++lan)
                {
                    if (lan != languageIndex)
                        UnloadLanguage(lan);
                }
            }
        }

        // if forceLoad, then the language is loaded from the cache even if its already loaded
        // this is needed to cleanup fallbacks
        public void LoadAllLanguages(bool forceLoad=false)
        {
            for (int i = 0; i < mLanguages.Count; ++i)
            {
                LoadLanguage(i, false, false, false, forceLoad);
            }
        }

        public void UnloadLanguage(int languageIndex)
        {
            if (!AllowUnloadingLanguages())
                return;

            // Some consoles don't allow IO access
            if (!PersistentStorage.CanAccessFiles())
                return;

            if (!I2Utils.IsPlaying() ||
                !mLanguages[languageIndex].IsLoaded() ||
                !mLanguages[languageIndex].CanBeUnloaded() ||
                IsCurrentLanguage(languageIndex) ||
                !PersistentStorage.HasFile(PersistentStorage.EFileType.Temporal, GetSavedLanguageFileName(languageIndex)))
            {
                return;
            }

            foreach (var termData in mTerms)
            {
                termData.languages[languageIndex] = null;
            }
            mLanguages[languageIndex].SetLoaded(false);
        }

        public void SaveLanguages( bool unloadAll, PersistentStorage.EFileType fileLocation = PersistentStorage.EFileType.Temporal)
        {
            if (!AllowUnloadingLanguages())
                return;

            // Some consoles don't allow IO access
            if (!PersistentStorage.CanAccessFiles())
                return;

            for (int i = 0; i < mLanguages.Count; ++i)
            {
                var data = Export_Language_to_Cache(i, IsCurrentLanguage(i));
                if (string.IsNullOrEmpty(data))
                    continue;

                PersistentStorage.SaveFile(PersistentStorage.EFileType.Temporal, GetSavedLanguageFileName(i), data);
            }

            if (unloadAll)
            {
                for (int i = 0; i < mLanguages.Count; ++i)
                {
                    if (unloadAll && !IsCurrentLanguage(i))
                        UnloadLanguage(i);
                }
            }
        }

        public bool HasUnloadedLanguages()
        {
            for (int i = 0; i < mLanguages.Count; ++i)
            {
                if (!mLanguages[i].IsLoaded())
                    return true;
            }
            return false;

        }
#endregion
    }
}