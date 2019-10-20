using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace I2.Loc
{
    public static partial class LocalizationManager
    {
        #region Variables: CurrentLanguage

        public static string CurrentLanguage
        {
            get {
                InitializeIfNeeded();
                return _mCurrentLanguage;
            }
            set {
                InitializeIfNeeded();
                string supportedLanguage = GetSupportedLanguage(value);
                if (!string.IsNullOrEmpty(supportedLanguage) && _mCurrentLanguage != supportedLanguage)
                {
                    SetLanguageAndCode(supportedLanguage, GetLanguageCode(supportedLanguage));
                }
            }
        }
        public static string CurrentLanguageCode
        {
            get {
                InitializeIfNeeded();
                return _mLanguageCode; }
            set {
                InitializeIfNeeded();
                if (_mLanguageCode != value)
                {
                    string lanName = GetLanguageFromCode(value);
                    if (!string.IsNullOrEmpty(lanName))
                        SetLanguageAndCode(lanName, value);
                }
            }
        }

        // "English (United States)" (get returns "United States") 
        // when set "Canada", the new language code will be "English (Canada)"
        public static string CurrentRegion
        {
            get {
                var lan = CurrentLanguage;
                int idx = lan.IndexOfAny("/\\".ToCharArray());
                if (idx > 0)
                    return lan.Substring(idx + 1);

                idx = lan.IndexOfAny("[(".ToCharArray());
                int idx2 = lan.LastIndexOfAny("])".ToCharArray());
                if (idx > 0 && idx != idx2)
                    return lan.Substring(idx + 1, idx2 - idx - 1);
                else
                    return string.Empty;
            }
            set {
                var lan = CurrentLanguage;
                int idx = lan.IndexOfAny("/\\".ToCharArray());
                if (idx > 0)
                {
                    CurrentLanguage = lan.Substring(idx + 1) + value;
                    return;
                }

                idx = lan.IndexOfAny("[(".ToCharArray());
                int idx2 = lan.LastIndexOfAny("])".ToCharArray());
                if (idx > 0 && idx != idx2)
                    lan = lan.Substring(idx);

                CurrentLanguage = lan + "(" + value + ")";
            }
        }

        // "en-US" (get returns "US") (when set "CA", the new language code will be "en-CA")
        public static string CurrentRegionCode
        {
            get {
                var code = CurrentLanguageCode;
                int idx = code.IndexOfAny(" -_/\\".ToCharArray());
                return idx < 0 ? string.Empty : code.Substring(idx + 1);
            }
            set {
                var code = CurrentLanguageCode;
                int idx = code.IndexOfAny(" -_/\\".ToCharArray());
                if (idx > 0)
                    code = code.Substring(0, idx);

                CurrentLanguageCode = code + "-" + value;
            }
        }

        public static CultureInfo CurrentCulture
        {
            get
            {
                return _mCurrentCulture;
            }
        }

        static string _mCurrentLanguage;
        static string _mLanguageCode;
        static CultureInfo _mCurrentCulture;
        static bool _mChangeCultureInfo = false;

        public static bool IsRight2Left = false;
        public static bool HasJoinedWords = false;  // Some languages (e.g. Chinese, Japanese and Thai) don't add spaces to their words (all characters are placed toguether)

        #endregion

        public static void SetLanguageAndCode(string languageName, string languageCode, bool rememberLanguage = true, bool force = false)
        {
            if (_mCurrentLanguage != languageName || _mLanguageCode != languageCode || force)
            {
                if (rememberLanguage)
                    PersistentStorage.SetSetting_String("I2 Language", languageName);
                _mCurrentLanguage = languageName;
                _mLanguageCode = languageCode;
                _mCurrentCulture = CreateCultureForCode(languageCode);
                if (_mChangeCultureInfo)
                    SetCurrentCultureInfo();

                IsRight2Left = IsRtl(_mLanguageCode);
                HasJoinedWords = GoogleLanguages.LanguageCode_HasJoinedWord(_mLanguageCode);
                LocalizeAll(force);
            }
        }

        static CultureInfo CreateCultureForCode(string code)
        {
#if !NETFX_CORE
            try
            {
                return CultureInfo.CreateSpecificCulture(code);
            }
            catch (System.Exception)
            {
                return CultureInfo.InvariantCulture;
            }
#else
			return CultureInfo.InvariantCulture;
#endif
        }

        public static void EnableChangingCultureInfo(bool bEnable)
        {
            if (!_mChangeCultureInfo && bEnable)
                SetCurrentCultureInfo();
            _mChangeCultureInfo = bEnable;
        }

        static void SetCurrentCultureInfo()
        {
            #if !NETFX_CORE
                System.Threading.Thread.CurrentThread.CurrentCulture = _mCurrentCulture;
            #endif
        }


        static void SelectStartupLanguage()
        {
			if (Sources.Count == 0)
				return;

            // Use the system language if there is a source with that language, 
            // or pick any of the languages provided by the sources

            string savedLanguage = PersistentStorage.GetSetting_String("I2 Language", string.Empty);
            string sysLanguage = GetCurrentDeviceLanguage();

            // Try selecting the System Language
            // But fallback to the first language found  if the System Language is not available in any source

			if (!string.IsNullOrEmpty(savedLanguage) && HasLanguage(savedLanguage, initialize: false, skipDisabled:true))
            {
                SetLanguageAndCode(savedLanguage, GetLanguageCode(savedLanguage));
                return;
            }

			if (!Sources [0].ignoreDeviceLanguage) 
			{
				// Check if the device language is supported. 
				// Also recognize when not region is set ("English (United State") will be used if sysLanguage is "English")
				string validLanguage = GetSupportedLanguage (sysLanguage, true);
				if (!string.IsNullOrEmpty (validLanguage)) {
					SetLanguageAndCode (validLanguage, GetLanguageCode (validLanguage), false);
					return;
				}
			}

            //--[ Use first language that its not disabled ]-----------
            for (int i = 0, imax = Sources.Count; i < imax; ++i)
                if (Sources[i].mLanguages.Count > 0)
                {
                    for (int j = 0; j < Sources[i].mLanguages.Count; ++j)
                        if (Sources[i].mLanguages[j].IsEnabled())
                        {
                            SetLanguageAndCode(Sources[i].mLanguages[j].name, Sources[i].mLanguages[j].code, false);
                            return;
                        }
                }
        }

 
		public static bool HasLanguage( string language, bool allowDiscartingRegion = true, bool initialize=true, bool skipDisabled=true )
		{
			if (initialize)
				InitializeIfNeeded();

			// First look for an exact match
			for (int i=0, imax=Sources.Count; i<imax; ++i)
				if (Sources[i].GetLanguageIndex(language, false, skipDisabled) >=0)
					return true;

			// Then allow matching "English (Canada)" to "english"
			if (allowDiscartingRegion)
			{
				for (int i=0, imax=Sources.Count; i<imax; ++i)
					if (Sources[i].GetLanguageIndex(language, true, skipDisabled) >=0)
						return true;
			}
			return false;
		}

		// Returns the provided language or a similar one without the Region 
		//(e.g. "English (Canada)" could be mapped to "english" or "English (United States)" if "English (Canada)" is not found
		public static string GetSupportedLanguage( string language, bool ignoreDisabled=false )
		{
            // First try finding the language that matches one of the official languages
            string code = GoogleLanguages.GetLanguageCode(language, false);
            if (!string.IsNullOrEmpty(code))
            {
                // First try finding if the exact language code is in one source
                for (int i = 0, imax = Sources.Count; i < imax; ++i)
                {
                    int idx = Sources[i].GetLanguageIndexFromCode(code, true, ignoreDisabled);
                    if (idx >= 0)
                        return Sources[i].mLanguages[idx].name;
                }

                // If not, try checking without the region
                for (int i = 0, imax = Sources.Count; i < imax; ++i)
                {
                    int idx = Sources[i].GetLanguageIndexFromCode(code, false, ignoreDisabled);
                    if (idx >= 0)
                        return Sources[i].mLanguages[idx].name;
                }
            }

            // If not found, then try finding an exact match for the name
            for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				int idx = Sources[i].GetLanguageIndex(language, false, ignoreDisabled);
				if (idx>=0)
					return Sources[i].mLanguages[idx].name;
			}
			
			// Then allow matching "English (Canada)" to "english"
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				int idx = Sources[i].GetLanguageIndex(language, true, ignoreDisabled);
				if (idx>=0)
					return Sources[i].mLanguages[idx].name;
			}

			return string.Empty;
		}

		public static string GetLanguageCode( string language )
		{
			if (Sources.Count==0)
				UpdateSources();
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				int idx = Sources[i].GetLanguageIndex(language);
				if (idx>=0)
					return Sources[i].mLanguages[idx].code;
			}
			return string.Empty;
		}

		public static string GetLanguageFromCode( string code, bool exactMatch=true )
		{
			if (Sources.Count==0)
				UpdateSources();
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				int idx = Sources[i].GetLanguageIndexFromCode(code, exactMatch);
				if (idx>=0)
					return Sources[i].mLanguages[idx].name;
			}
			return string.Empty;
		}


		public static List<string> GetAllLanguages ( bool skipDisabled = true )
		{
			if (Sources.Count==0)
				UpdateSources();
			List<string> languages = new List<string> ();
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				languages.AddRange(Sources[i].GetLanguages(skipDisabled).Where(x=>!languages.Contains(x)));
			}
			return languages;
		}

		public static List<string> GetAllLanguagesCode(bool allowRegions=true, bool skipDisabled = true)
		{
			List<string> languages = new List<string>();
			for (int i = 0, imax = Sources.Count; i < imax; ++i)
			{
				languages.AddRange(Sources[i].GetLanguagesCode(allowRegions, skipDisabled).Where(x => !languages.Contains(x)));
			}
			return languages;
		}

		public static bool IsLanguageEnabled(string language)
		{
			for (int i = 0, imax = Sources.Count; i < imax; ++i)
				if (!Sources[i].IsLanguageEnabled(language))
					return false;
			return true;
		}

        static void LoadCurrentLanguage()
        {
            for (int i = 0; i < Sources.Count; ++i)
            {
                var iCurrentLang = Sources[i].GetLanguageIndex(_mCurrentLanguage, true, false);
                Sources[i].LoadLanguage(iCurrentLang, true, true, true, false);
            }
        }


        // This function should only be called from within the Localize Inspector to temporaly preview that Language
        public static void PreviewLanguage(string newLanguage)
		{
			_mCurrentLanguage = newLanguage;
			_mLanguageCode = GetLanguageCode(_mCurrentLanguage);
			IsRight2Left = IsRtl(_mLanguageCode);
            HasJoinedWords = GoogleLanguages.LanguageCode_HasJoinedWord(_mLanguageCode);
        }
    }
}
