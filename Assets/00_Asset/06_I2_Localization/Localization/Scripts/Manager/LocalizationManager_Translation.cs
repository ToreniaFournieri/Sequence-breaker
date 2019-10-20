using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Collections;

namespace I2.Loc
{
    public static partial class LocalizationManager
    {

        #region Variables: Misc

        public delegate void OnLocalizeCallback();
        public static event OnLocalizeCallback OnLocalizeEvent;

        static bool _mLocalizeIsScheduled = false;
        static bool _mLocalizeIsScheduledWithForcedValue = false;

        public static bool HighlightLocalizedTargets = false;


        #endregion

        public static string GetTranslation(string term, bool fixForRtl = true, int maxLineLengthForRtl = 0, bool ignoreRtLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
        {
            string translation = null;
            TryGetTranslation(term, out translation, fixForRtl, maxLineLengthForRtl, ignoreRtLnumbers, applyParameters, localParametersRoot, overrideLanguage);

            return translation;
        }
        public static string GetTermTranslation(string term, bool fixForRtl = true, int maxLineLengthForRtl = 0, bool ignoreRtLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
        {
            return GetTranslation(term, fixForRtl, maxLineLengthForRtl, ignoreRtLnumbers, applyParameters, localParametersRoot, overrideLanguage);
        }


        public static bool TryGetTranslation(string term, out string translation, bool fixForRtl = true, int maxLineLengthForRtl = 0, bool ignoreRtLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
        {
            translation = null;
            if (string.IsNullOrEmpty(term))
                return false;

            InitializeIfNeeded();

            for (int i = 0, imax = Sources.Count; i < imax; ++i)
            {
                if (Sources[i].TryGetTranslation(term, out translation, overrideLanguage))
                {
                    if (applyParameters)
                        ApplyLocalizationParams(ref translation, localParametersRoot, allowLocalizedParameters:true);

                    if (IsRight2Left && fixForRtl)
                        translation = ApplyRtLfix(translation, maxLineLengthForRtl, ignoreRtLnumbers);
                    return true;
                }
            }

            return false;
        }

        public static T GetTranslatedObject<T>( string assetName, Localize optionalLocComp=null) where T : Object
        {
            if (optionalLocComp != null)
            {
                return optionalLocComp.FindTranslatedObject<T>(assetName);
            }
            else
            {
                T obj = FindAsset(assetName) as T;
                if (obj)
                    return obj;

                obj = ResourceManager.PInstance.GetAsset<T>(assetName);
                return obj;
            }
        }
        
        public static T GetTranslatedObjectByTermName<T>( string term, Localize optionalLocComp=null) where T : Object
        {
            string    translation = GetTranslation(term, fixForRtl: false);
            return GetTranslatedObject<T>(translation);
        }
        

        public static string GetAppName(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode))
            {
                for (int i = 0; i < Sources.Count; ++i)
                {
                    if (string.IsNullOrEmpty(Sources[i].mTermAppName))
                        continue;

                    int langIdx = Sources[i].GetLanguageIndexFromCode(languageCode, false);
                    if (langIdx < 0)
                        continue;

                    var termData = Sources[i].GetTermData(Sources[i].mTermAppName);
                    if (termData == null)
                        continue;

                    var appName = termData.GetTranslation(langIdx);
                    if (!string.IsNullOrEmpty(appName))
                        return appName;
                }
            }

            return Application.productName;
        }

        public static void LocalizeAll(bool force = false)
		{
            LoadCurrentLanguage();

            if (!Application.isPlaying)
			{
				DoLocalizeAll(force);
				return;
			}
			_mLocalizeIsScheduledWithForcedValue |= force;
            if (_mLocalizeIsScheduled)
            {
                return;
            }
			I2.Loc.CoroutineManager.Start(Coroutine_LocalizeAll());
		}

		static IEnumerator Coroutine_LocalizeAll()
		{
			_mLocalizeIsScheduled = true;
            yield return null;
            _mLocalizeIsScheduled = false;
            var force = _mLocalizeIsScheduledWithForcedValue;
			_mLocalizeIsScheduledWithForcedValue = false;
			DoLocalizeAll(force);
		}

		static void DoLocalizeAll(bool force = false)
		{
			Localize[] locals = (Localize[])Resources.FindObjectsOfTypeAll( typeof(Localize) );
			for (int i=0, imax=locals.Length; i<imax; ++i)
			{
				Localize local = locals[i];
				//if (ObjectExistInScene (local.gameObject))
				local.OnLocalize(force);
			}
			if (OnLocalizeEvent != null)
				OnLocalizeEvent ();
			//ResourceManager.pInstance.CleanResourceCache();
            #if UNITY_EDITOR
                RepaintInspectors();
            #endif
        }

        #if UNITY_EDITOR
        static void RepaintInspectors()
        {
            var assemblyEditor = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var typeInspectorWindow = assemblyEditor.GetType("UnityEditor.InspectorWindow");
            if (typeInspectorWindow != null)
            {
                typeInspectorWindow.GetMethod("RepaintAllInspectors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, null);
            }
        }
        #endif


        public static List<string> GetCategories ()
		{
			List<string> categories = new List<string> ();
			for (int i=0, imax=Sources.Count; i<imax; ++i)
				Sources[i].GetCategories(false, categories);
			return categories;
		}



		public static List<string> GetTermsList ( string category = null )
		{
			if (Sources.Count==0)
				UpdateSources();

			if (Sources.Count==1)
				return Sources[0].GetTermsList(category);

			HashSet<string> terms = new HashSet<string> ();
			for (int i=0, imax=Sources.Count; i<imax; ++i)
				terms.UnionWith( Sources[i].GetTermsList(category) );
			return new List<string>(terms);
		}

		public static TermData GetTermData( string term )
		{
            InitializeIfNeeded();

			TermData data;
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				data = Sources[i].GetTermData(term);
				if (data!=null)
					return data;
			}

			return null;
		}
        public static TermData GetTermData(string term, out LanguageSourceData source)
        {
            InitializeIfNeeded();

            TermData data;
            for (int i = 0, imax = Sources.Count; i < imax; ++i)
            {
                data = Sources[i].GetTermData(term);
                if (data != null)
                {
                    source = Sources[i];
                    return data;
                }
            }

            source = null;
            return null;
        }

    }
}
