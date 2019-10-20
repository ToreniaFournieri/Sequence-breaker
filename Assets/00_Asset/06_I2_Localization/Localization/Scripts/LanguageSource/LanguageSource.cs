using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.LanguageSource
{
    [AddComponentMenu("I2/Localization/Source")]
    [ExecuteInEditMode]
	public class LanguageSource : MonoBehaviour, ISerializationCallbackReceiver, ILanguageSource
    {
        public LanguageSourceData SourceData
        {
            get { return mSource; }
            set { mSource = value; }
        }
        public LanguageSourceData mSource = new LanguageSourceData();

        // Because of Unity2018.3 change in Prefabs, now all the source variables are moved into LanguageSourceData
        // But to avoid loosing previously serialized data, these vars are copied into mSource.XXXX when deserializing)
        // These are going to be removed once everyone port their projects to the new I2L version.
        #region Legacy Variables 

        // TODO: also copy         public string name;   and owner

        public int version = 0;
        [FormerlySerializedAs("NeverDestroy")] public bool neverDestroy = false;  	// Keep between scenes (will call DontDestroyOnLoad )

		[FormerlySerializedAs("UserAgreesToHaveItOnTheScene")] public bool userAgreesToHaveItOnTheScene = false;
		[FormerlySerializedAs("UserAgreesToHaveItInsideThePluginsFolder")] public bool userAgreesToHaveItInsideThePluginsFolder = false;
        [FormerlySerializedAs("GoogleLiveSyncIsUptoDate")] public bool googleLiveSyncIsUptoDate = true;

        [FormerlySerializedAs("Assets")] public List<Object> assets = new List<Object>();	// References to Fonts, Atlasses and other objects the localization may need

        [FormerlySerializedAs("Google_WebServiceURL")] public string googleWebServiceUrl;
        [FormerlySerializedAs("Google_SpreadsheetKey")] public string googleSpreadsheetKey;
        [FormerlySerializedAs("Google_SpreadsheetName")] public string googleSpreadsheetName;
        [FormerlySerializedAs("Google_LastUpdatedVersion")] public string googleLastUpdatedVersion;


        [FormerlySerializedAs("GoogleUpdateFrequency")] public LanguageSourceData.EGoogleUpdateFrequency googleUpdateFrequency = LanguageSourceData.EGoogleUpdateFrequency.Weekly;

        [FormerlySerializedAs("GoogleUpdateDelay")] public float googleUpdateDelay = 5; // How many second to delay downloading data from google (to avoid lag on the startup)

        public delegate void FnOnSourceUpdated(LanguageSourceData source, bool receivedNewData, string errorMsg);
        public event FnOnSourceUpdated EventOnSourceUpdateFromGoogle;

        public List<LanguageData> mLanguages = new List<LanguageData>();

        [FormerlySerializedAs("IgnoreDeviceLanguage")] public bool ignoreDeviceLanguage; // If false, it will use the Device's language as the initial Language, otherwise it will use the first language in the source.

        [FormerlySerializedAs("_AllowUnloadingLanguages")] public LanguageSourceData.EAllowUnloadLanguages allowUnloadingLanguages = LanguageSourceData.EAllowUnloadLanguages.Never;

        public List<TermData> mTerms = new List<TermData>();

        [FormerlySerializedAs("CaseInsensitiveTerms")] public bool caseInsensitiveTerms = false;

        [FormerlySerializedAs("OnMissingTranslation")] public LanguageSourceData.MissingTranslationAction onMissingTranslation = LanguageSourceData.MissingTranslationAction.Fallback;

        [FormerlySerializedAs("mTerm_AppName")] public string mTermAppName;

        #endregion

        #region EditorVariables
        #if UNITY_EDITOR

            [FormerlySerializedAs("Spreadsheet_LocalFileName")] public string spreadsheetLocalFileName;
		    [FormerlySerializedAs("Spreadsheet_LocalCSVSeparator")] public string spreadsheetLocalCsvSeparator = ",";
            [FormerlySerializedAs("Spreadsheet_LocalCSVEncoding")] public string spreadsheetLocalCsvEncoding = "utf-8";
            [FormerlySerializedAs("Spreadsheet_SpecializationAsRows")] public bool spreadsheetSpecializationAsRows = true;

            [FormerlySerializedAs("Google_Password")] public string googlePassword = "change_this";
            [FormerlySerializedAs("GoogleInEditorCheckFrequency")] public LanguageSourceData.EGoogleUpdateFrequency googleInEditorCheckFrequency = LanguageSourceData.EGoogleUpdateFrequency.Daily;
#endif
        #endregion

        void Awake()
        {
            #if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
                return;
            #endif
   //         NeverDestroy = false;

   //         if (NeverDestroy)
			//{
			//	if (mSource.ManagerHasASimilarSource())
			//	{
			//		Object.Destroy (this);
			//		return;
			//	}
			//	else
			//	{
			//		if (Application.isPlaying)
			//			DontDestroyOnLoad (gameObject);
			//	}
			//}
            mSource.Owner = this;
            mSource.Awake();
        }

        private void OnDestroy()
        {
            neverDestroy = false;

            if (!neverDestroy)
            {
               mSource.OnDestroy();
            }
        }

        public string GetSourceName()
        {
            string s = gameObject.name;
            Transform tr = transform.parent;
            while (tr)
            {
                s = string.Concat(tr.name, "_", s);
                tr = tr.parent;
            }
            return s;
        }

        public void OnBeforeSerialize()
        {
            version = 1;
        }

        public void OnAfterDeserialize()
        {
            if (version==0 || mSource==null)
            {
                mSource = new LanguageSourceData();
                mSource.Owner = this;
                mSource.userAgreesToHaveItOnTheScene = userAgreesToHaveItOnTheScene;
                mSource.userAgreesToHaveItInsideThePluginsFolder = userAgreesToHaveItInsideThePluginsFolder;
                mSource.ignoreDeviceLanguage = ignoreDeviceLanguage;
                mSource.allowUnloadingLanguages = allowUnloadingLanguages;
                mSource.caseInsensitiveTerms = caseInsensitiveTerms;
                mSource.onMissingTranslation = onMissingTranslation;
                mSource.mTermAppName = mTermAppName;

                mSource.googleLiveSyncIsUptoDate = googleLiveSyncIsUptoDate;
                mSource.googleWebServiceUrl = googleWebServiceUrl;
                mSource.googleSpreadsheetKey = googleSpreadsheetKey;
                mSource.googleSpreadsheetName = googleSpreadsheetName;
                mSource.googleLastUpdatedVersion = googleLastUpdatedVersion;
                mSource.googleUpdateFrequency = googleUpdateFrequency;
                mSource.googleUpdateDelay = googleUpdateDelay;
                
                mSource.EventOnSourceUpdateFromGoogle += EventOnSourceUpdateFromGoogle;

                if (mLanguages != null && mLanguages.Count>0)
                {
                    mSource.mLanguages.Clear();
                    mSource.mLanguages.AddRange(mLanguages);
                    mLanguages.Clear();
                }
                if (assets != null && assets.Count > 0)
                {
                    mSource.assets.Clear();
                    mSource.assets.AddRange(assets);
                    assets.Clear();
                }
                if (mTerms != null && mTerms.Count>0)
                {
                    mSource.mTerms.Clear();
                    for (int i=0; i<mTerms.Count; ++i)
                        mSource.mTerms.Add(mTerms[i]);
                    mTerms.Clear();
                }

                version = 1;

                EventOnSourceUpdateFromGoogle = null;
            }
        }
    }
}