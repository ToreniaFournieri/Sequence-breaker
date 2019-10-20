using System;
using System.Collections.Generic;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.LanguageSource
{
    public interface ILanguageSource
    {
        LanguageSourceData SourceData { get; set; }
    }

    [ExecuteInEditMode]
    [Serializable]
	public partial class LanguageSourceData
    {
        #region Variables

        [NonSerialized] public ILanguageSource Owner;
        public Object OwnerObject { get { return Owner as UnityEngine.Object; } }

        [FormerlySerializedAs("UserAgreesToHaveItOnTheScene")] public bool userAgreesToHaveItOnTheScene = false;
		[FormerlySerializedAs("UserAgreesToHaveItInsideThePluginsFolder")] public bool userAgreesToHaveItInsideThePluginsFolder = false;
        [FormerlySerializedAs("GoogleLiveSyncIsUptoDate")] public bool googleLiveSyncIsUptoDate = true;

        [NonSerialized] public bool MIsGlobalSource;

        #endregion

        #region Variables : Terms

        public List<TermData> mTerms = new List<TermData>();

        [FormerlySerializedAs("CaseInsensitiveTerms")] public bool caseInsensitiveTerms = false;

        //This is used to overcome the issue with Unity not serializing Dictionaries
        [NonSerialized] public Dictionary<string, TermData> MDictionary = new Dictionary<string, TermData>(StringComparer.Ordinal);

        public enum MissingTranslationAction { Empty, Fallback, ShowWarning, ShowTerm };
        [FormerlySerializedAs("OnMissingTranslation")] public MissingTranslationAction onMissingTranslation = MissingTranslationAction.Fallback;

        [FormerlySerializedAs("mTerm_AppName")] public string mTermAppName;

        #endregion

        #region Variables : Languages

        public List<LanguageData> mLanguages = new List<LanguageData>();

        [FormerlySerializedAs("IgnoreDeviceLanguage")] public bool ignoreDeviceLanguage; // If false, it will use the Device's language as the initial Language, otherwise it will use the first language in the source.

        public enum EAllowUnloadLanguages { Never, OnlyInDevice, EditorAndDevice }
        [FormerlySerializedAs("_AllowUnloadingLanguages")] public EAllowUnloadLanguages allowUnloadingLanguages = EAllowUnloadLanguages.Never;

        #endregion

        #region Variables : Google

        [FormerlySerializedAs("Google_WebServiceURL")] public string googleWebServiceUrl;
        [FormerlySerializedAs("Google_SpreadsheetKey")] public string googleSpreadsheetKey;
        [FormerlySerializedAs("Google_SpreadsheetName")] public string googleSpreadsheetName;
        [FormerlySerializedAs("Google_LastUpdatedVersion")] public string googleLastUpdatedVersion;

#if UNITY_EDITOR
        [FormerlySerializedAs("Google_Password")] public string googlePassword = "change_this";
#endif

        public enum EGoogleUpdateFrequency { Always, Never, Daily, Weekly, Monthly, OnlyOnce, EveryOtherDay }
        [FormerlySerializedAs("GoogleUpdateFrequency")] public EGoogleUpdateFrequency googleUpdateFrequency = EGoogleUpdateFrequency.Weekly;
        [FormerlySerializedAs("GoogleInEditorCheckFrequency")] public EGoogleUpdateFrequency googleInEditorCheckFrequency = EGoogleUpdateFrequency.Daily;

        // When Manual, the user has to call LocalizationManager.ApplyDownloadedDataFromGoogle() during a loading screen or similar
        public enum EGoogleUpdateSynchronization { Manual, OnSceneLoaded, AsSoonAsDownloaded }
        [FormerlySerializedAs("GoogleUpdateSynchronization")] public EGoogleUpdateSynchronization googleUpdateSynchronization = EGoogleUpdateSynchronization.OnSceneLoaded;

        [FormerlySerializedAs("GoogleUpdateDelay")] public float googleUpdateDelay = 0; // How many second to delay downloading data from google (to avoid lag on the startup)

        public event LanguageSource.FnOnSourceUpdated EventOnSourceUpdateFromGoogle;    // (LanguageSource, bool ReceivedNewData, string errorMsg)

        #endregion

        #region Variables : Assets

        [FormerlySerializedAs("Assets")] public List<Object> assets = new List<Object>();	// References to Fonts, Atlasses and other objects the localization may need

        //This is used to overcome the issue with Unity not serializing Dictionaries
        [NonSerialized] public Dictionary<string, Object> MAssetDictionary = new Dictionary<string, Object>(StringComparer.Ordinal);

        #endregion

        #region EditorVariables
#if UNITY_EDITOR

        [FormerlySerializedAs("Spreadsheet_LocalFileName")] public string spreadsheetLocalFileName;
		[FormerlySerializedAs("Spreadsheet_LocalCSVSeparator")] public string spreadsheetLocalCsvSeparator = ",";
        [FormerlySerializedAs("Spreadsheet_LocalCSVEncoding")] public string spreadsheetLocalCsvEncoding = "utf-8";
        [FormerlySerializedAs("Spreadsheet_SpecializationAsRows")] public bool spreadsheetSpecializationAsRows = true;

#endif
        #endregion

        #region Language

        public void Awake()
		{
			LocalizationManager.AddSource (this);
			UpdateDictionary();
            UpdateAssetDictionary();
            LocalizationManager.LocalizeAll(true);
        }

        public void OnDestroy()
        {
            LocalizationManager.RemoveSource(this);
        }
 


		public bool IsEqualTo( LanguageSourceData source )
		{
			if (source.mLanguages.Count != mLanguages.Count)
				return false;

			for (int i=0, imax=mLanguages.Count; i<imax; ++i)
				if (source.GetLanguageIndex( mLanguages[i].name ) < 0)
					return false;

			if (source.mTerms.Count != mTerms.Count)
				return false;

			for (int i=0; i<mTerms.Count; ++i)
				if (source.GetTermData(mTerms[i].term)==null)
					return false;

			return true;
		}

		internal bool ManagerHasASimilarSource()
		{
			for (int i=0, imax=LocalizationManager.Sources.Count; i<imax; ++i)
			{
				LanguageSourceData source = (LocalizationManager.Sources[i] as LanguageSourceData);
				if (source!=null && source.IsEqualTo(this) && source!=this)
					return true;
			}
			return false;
		}

		public void ClearAllData()
		{
			mTerms.Clear ();
			mLanguages.Clear ();
			MDictionary.Clear();
            MAssetDictionary.Clear();
		}

        public bool IsGlobalSource()
        {
            return MIsGlobalSource;
        }

        #endregion

        public void Editor_SetDirty()
        {
            #if UNITY_EDITOR
                if (OwnerObject != null)
                {
                    UnityEditor.EditorUtility.SetDirty(OwnerObject);
                }
            #endif
        }

    }
}