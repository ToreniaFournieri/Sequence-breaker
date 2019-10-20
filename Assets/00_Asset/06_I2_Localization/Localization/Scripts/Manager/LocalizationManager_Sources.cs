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

        public static List<LanguageSourceData> Sources = new List<LanguageSourceData>();
        public static string[] GlobalSources = { "I2Languages" };

        #endregion

        #region Sources

        public static bool UpdateSources()
		{
			UnregisterDeletededSources();
			RegisterSourceInResources();
			RegisterSceneSources();
			return Sources.Count>0;
		}

		static void UnregisterDeletededSources()
		{
			// Delete sources that were part of another scene and not longer available
			for (int i=Sources.Count-1; i>=0; --i)
				if (Sources[i] == null)
					RemoveSource( Sources[i] );
		}

		static void RegisterSceneSources()
		{
			LanguageSource[] sceneSources = (LanguageSource[])Resources.FindObjectsOfTypeAll( typeof(LanguageSource) );
            foreach (LanguageSource source in sceneSources)
				if (!Sources.Contains(source.mSource))
				{
                    if (source.mSource.Owner == null)
                        source.mSource.Owner = source;
                    AddSource( source.mSource );
				}
		}		

		static void RegisterSourceInResources()
		{
			// Find the Source that its on the Resources Folder
			foreach (string sourceName in GlobalSources)
			{
				LanguageSourceAsset sourceAsset = (ResourceManager.PInstance.GetAsset<LanguageSourceAsset>(sourceName));
				
				if (sourceAsset && !Sources.Contains(sourceAsset.mSource))
                {
                    if (!sourceAsset.mSource.MIsGlobalSource)
                        sourceAsset.mSource.MIsGlobalSource = true;
                    sourceAsset.mSource.Owner = sourceAsset;
                    AddSource(sourceAsset.mSource);
                }
            }
		}		

		internal static void AddSource ( LanguageSourceData source )
		{
			if (Sources.Contains (source))
				return;

            Sources.Add( source );

			if (source.HasGoogleSpreadsheet() && source.googleUpdateFrequency != LanguageSourceData.EGoogleUpdateFrequency.Never)
			{
                #if !UNITY_EDITOR
                    Source.Import_Google_FromCache();
                    bool justCheck = false;
                #else
                    bool justCheck=true;
                #endif
                if (source.googleUpdateDelay > 0)
						CoroutineManager.Start( Delayed_Import_Google(source, source.googleUpdateDelay, justCheck) );
				else
					source.Import_Google(false, justCheck);
            }

            //if (force)
            {
                for (int i = 0; i < source.mLanguages.Count(); ++i)
                    source.mLanguages[i].SetLoaded(true);
            }

            if (source.MDictionary.Count==0)
				source.UpdateDictionary(true);
		}

		static IEnumerator Delayed_Import_Google ( LanguageSourceData source, float delay, bool justCheck )
		{
			yield return new WaitForSeconds( delay );
            if (source != null) // handle cases where the source is already deleted
            {
                source.Import_Google(false, justCheck);
            }
		}

		internal static void RemoveSource (LanguageSourceData source )
		{
			//Debug.Log ("RemoveSource " + Source+" " + Source.GetInstanceID());
			Sources.Remove( source );
		}

		public static bool IsGlobalSource( string sourceName )
		{
			return System.Array.IndexOf(GlobalSources, sourceName)>=0;
		}

		public static LanguageSourceData GetSourceContaining( string term, bool fallbackToFirst = true )
		{
			if (!string.IsNullOrEmpty(term))
			{
				for (int i=0, imax=Sources.Count; i<imax; ++i)
				{
					if (Sources[i].GetTermData(term) != null)
						return Sources[i];
				}
			}
			
			return ((fallbackToFirst && Sources.Count>0) ? Sources[0] :  null);
		}

		public static Object FindAsset (string value)
		{
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				Object obj = Sources[i].FindAsset(value);
				if (obj)
					return obj;
			}
			return null;
		}

        public static void ApplyDownloadedDataFromGoogle()
        {
            for (int i = 0, imax = Sources.Count; i < imax; ++i)
            {
                Sources[i].ApplyDownloadedDataFromGoogle();
            }
        }

        #endregion

    }
}
