using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace I2.Loc
{
	public partial class LanguageSourceData
	{
        private string _mDelayedGoogleData;  // Data that was downloaded and is waiting for a levelLoaded event to apply the localization without a lag in performance
		#region Connection to Web Service 

		public static void FreeUnusedLanguages()
		{
			var source    = LocalizationManager.Sources[0];
			int langIndex = source.GetLanguageIndex(LocalizationManager.CurrentLanguage);

			for (int i=0; i<source.mTerms.Count; ++i)
			{
				var term = source.mTerms[i];
				for (int j=0; j<term.languages.Length; j++)
				{
					if (j != langIndex)
						term.languages[j] = null;
				}
			}
		}

		public void Import_Google_FromCache()
		{
			if (googleUpdateFrequency==EGoogleUpdateFrequency.Never)
				return;
			
			if (!I2Utils.IsPlaying())
					return;
					
			string playerPrefName = GetSourcePlayerPrefName();
			string i2SavedData = PersistentStorage.LoadFile(PersistentStorage.EFileType.Persistent, "I2Source_"+ playerPrefName + ".loc", false);
			if (string.IsNullOrEmpty (i2SavedData))
				return;

            if (i2SavedData.StartsWith("[i2e]", StringComparison.Ordinal))
            {
                i2SavedData = StringObfucator.Decode(i2SavedData.Substring(5, i2SavedData.Length-5));
            }

			//--[ Compare with current version ]-----
			bool shouldUpdate = false;
			string savedSpreadsheetVersion = googleLastUpdatedVersion;
			if (PersistentStorage.HasSetting("I2SourceVersion_"+playerPrefName))
			{
				savedSpreadsheetVersion = PersistentStorage.GetSetting_String("I2SourceVersion_"+playerPrefName, googleLastUpdatedVersion);
//				Debug.Log (Google_LastUpdatedVersion + " - " + savedSpreadsheetVersion);
				shouldUpdate = IsNewerVersion(googleLastUpdatedVersion, savedSpreadsheetVersion);
			}

			if (!shouldUpdate)
			{
                PersistentStorage.DeleteFile(PersistentStorage.EFileType.Persistent, "I2Source_"+playerPrefName+".loc", false);
                PersistentStorage.DeleteSetting("I2SourceVersion_"+playerPrefName);
				return;
			}

			if (savedSpreadsheetVersion.Length > 19) // Check for corruption from previous versions
				savedSpreadsheetVersion = string.Empty;
			googleLastUpdatedVersion = savedSpreadsheetVersion;

			//Debug.Log ("[I2Loc] Using Saved (PlayerPref) data in 'I2Source_"+PlayerPrefName+"'" );
			Import_Google_Result(i2SavedData, ESpreadsheetUpdateMode.Replace);
		}

		bool IsNewerVersion( string currentVersion, string newVersion )
		{
			if (string.IsNullOrEmpty (newVersion))			// if no new version
				return false;
			if (string.IsNullOrEmpty (currentVersion))		// there is a new version, but not a current one
				return true;
			
			long currentV, newV;
			if (!long.TryParse (newVersion, out newV) || !long.TryParse (currentVersion, out currentV))	// if can't parse either, then force get the new one
				return true;

			return newV > currentV;
		}

        // When JustCheck is true, importing from google will not download any data, just detect if the Spreadsheet is up-to-date
		public void Import_Google( bool forceUpdate, bool justCheck)
		{
            if (!forceUpdate && googleUpdateFrequency==EGoogleUpdateFrequency.Never)
				return;

            if (!I2Utils.IsPlaying())
                return;

            #if UNITY_EDITOR
            if (justCheck && googleInEditorCheckFrequency==EGoogleUpdateFrequency.Never)
                return;
            #endif

            #if UNITY_EDITOR
                        var updateFrequency = googleInEditorCheckFrequency;
            #else
                        var updateFrequency = GoogleUpdateFrequency;
            #endif

            string playerPrefName = GetSourcePlayerPrefName();

            if (!forceUpdate && updateFrequency != EGoogleUpdateFrequency.Always)
			{
                #if UNITY_EDITOR
                    string sTimeOfLastUpdate = UnityEditor.EditorPrefs.GetString("LastGoogleUpdate_"+playerPrefName, "");
                #else
                    string sTimeOfLastUpdate = PersistentStorage.GetSetting_String("LastGoogleUpdate_"+PlayerPrefName, "");
                #endif
				DateTime timeOfLastUpdate;
				try
				{
					if (DateTime.TryParse( sTimeOfLastUpdate, out timeOfLastUpdate ))
					{
						double timeDifference = (DateTime.Now-timeOfLastUpdate).TotalDays;
                        switch (updateFrequency)
						{
							case EGoogleUpdateFrequency.Daily: if (timeDifference<1) return;
								break;
							case EGoogleUpdateFrequency.Weekly: if (timeDifference<8) return;
								break;
							case EGoogleUpdateFrequency.Monthly: if (timeDifference<31) return;
								break;
							case EGoogleUpdateFrequency.OnlyOnce: return;
							case EGoogleUpdateFrequency.EveryOtherDay : if (timeDifference < 2) return;
								break;
						}
					}
				}
				catch(Exception)
				{ }
			}
            #if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetString("LastGoogleUpdate_" + playerPrefName, DateTime.Now.ToString());
            #else
                PersistentStorage.SetSetting_String("LastGoogleUpdate_"+PlayerPrefName, DateTime.Now.ToString());
            #endif

			//--[ Checking google for updated data ]-----------------
			CoroutineManager.Start(Import_Google_Coroutine(justCheck));
		}

		string GetSourcePlayerPrefName()
		{
            if (Owner == null)
                return null;
            string sourceName = (Owner as UnityEngine.Object).name;
            if (!string.IsNullOrEmpty(googleSpreadsheetKey))
            {
                sourceName += googleSpreadsheetKey;
            }
            // If its a global source, use its name, otherwise, use the name and the level it is in
            if (Array.IndexOf(LocalizationManager.GlobalSources, (Owner as UnityEngine.Object).name)>=0)
				return sourceName;
			else
			{
#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				return Application.loadedLevelName + "_" + sourceName;
#else
                return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name+"_"+ sourceName;
#endif
			}
		}

		IEnumerator Import_Google_Coroutine(bool justCheck)
		{
            UnityWebRequest www = Import_Google_CreateWWWcall(false, justCheck);
			if (www==null)
				yield break;

			while (!www.isDone)
				yield return null;

			//Debug.Log ("Google Result: " + www.text);
			bool notError = string.IsNullOrEmpty(www.error);
			string wwwText = null;

			if (notError)
			{
				var bytes = www.downloadHandler.data;
				wwwText = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length); //www.text

                bool isEmpty = string.IsNullOrEmpty(wwwText) || wwwText == "\"\"";

                if (justCheck)
                {
                    if (!isEmpty)
                    {
                        Debug.LogWarning("Spreadsheet is not up-to-date and Google Live Synchronization is enabled\nWhen playing in the device the Spreadsheet will be downloaded and translations may not behave as what you see in the editor.\nTo fix this, Import or Export replace to Google");
                        googleLiveSyncIsUptoDate = false;
                    }

                    yield break;
                }

                if (!isEmpty)
                {
                    _mDelayedGoogleData = wwwText;

                    switch (googleUpdateSynchronization)
                    {
                        case EGoogleUpdateSynchronization.AsSoonAsDownloaded:
                            {
                                ApplyDownloadedDataFromGoogle();
                                break;
                            }
                        case EGoogleUpdateSynchronization.Manual:
                            break;
                        case EGoogleUpdateSynchronization.OnSceneLoaded:
                            {
                                SceneManager.sceneLoaded += ApplyDownloadedDataOnSceneLoaded;
                                break;
                            }
                    }

                    yield break;
                }
			}

			if (EventOnSourceUpdateFromGoogle != null)
				EventOnSourceUpdateFromGoogle(this, false, www.error);

			Debug.Log("Language Source was up-to-date with Google Spreadsheet");
		}

        void ApplyDownloadedDataOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= ApplyDownloadedDataOnSceneLoaded;
            ApplyDownloadedDataFromGoogle();
        }

        public void ApplyDownloadedDataFromGoogle()
        {
            if (string.IsNullOrEmpty(_mDelayedGoogleData))
                return;

            var errorMsg = Import_Google_Result(_mDelayedGoogleData, ESpreadsheetUpdateMode.Replace, true);
            if (string.IsNullOrEmpty(errorMsg))
            {
                if (EventOnSourceUpdateFromGoogle != null)
                    EventOnSourceUpdateFromGoogle(this, true, "");

                LocalizationManager.LocalizeAll(true);
                Debug.Log("Done Google Sync");
            }
            else
            {
                if (EventOnSourceUpdateFromGoogle != null)
                    EventOnSourceUpdateFromGoogle(this, false, "");

                Debug.Log("Done Google Sync: source was up-to-date");
            }
        }

		public UnityWebRequest Import_Google_CreateWWWcall( bool forceUpdate, bool justCheck )
		{
			if (!HasGoogleSpreadsheet())
				return null;

			string savedVersion = PersistentStorage.GetSetting_String("I2SourceVersion_"+GetSourcePlayerPrefName(), googleLastUpdatedVersion);
			if (savedVersion.Length > 19) // Check for corruption
				savedVersion= string.Empty;

#if !UNITY_EDITOR
            if (IsNewerVersion(savedVersion, Google_LastUpdatedVersion))
				Google_LastUpdatedVersion = savedVersion;
#endif

			string query =  string.Format("{0}?key={1}&action=GetLanguageSource&version={2}", 
										  LocalizationManager.GetWebServiceUrl(this),
										  googleSpreadsheetKey,
										  forceUpdate ? "0" : googleLastUpdatedVersion);
#if UNITY_EDITOR
            if (justCheck)
            {
                query += "&justcheck=true";
            }
#endif
            UnityWebRequest www = UnityWebRequest.Get(query);
            I2Utils.SendWebRequest(www);
            return www;
		}

		public bool HasGoogleSpreadsheet()
		{
            return !string.IsNullOrEmpty(googleWebServiceUrl) && !string.IsNullOrEmpty(googleSpreadsheetKey) &&
                   !string.IsNullOrEmpty(LocalizationManager.GetWebServiceUrl(this));
        }

		public string Import_Google_Result( string jsonString, ESpreadsheetUpdateMode updateMode, bool saveInPlayerPrefs = false )
		{
            try
            {
                string errorMsg = string.Empty;
                if (string.IsNullOrEmpty(jsonString) || jsonString == "\"\"")
                {
                    return errorMsg;
                }

                int idxV = jsonString.IndexOf("version=", StringComparison.Ordinal);
                int idxSv = jsonString.IndexOf("script_version=", StringComparison.Ordinal);
                if (idxV < 0 || idxSv < 0)
                {
                    return "Invalid Response from Google, Most likely the WebService needs to be updated";
                }

                idxV += "version=".Length;
                idxSv += "script_version=".Length;

                string newSpreadsheetVersion = jsonString.Substring(idxV, jsonString.IndexOf(",", idxV, StringComparison.Ordinal) - idxV);
                var scriptVersion = int.Parse(jsonString.Substring(idxSv, jsonString.IndexOf(",", idxSv, StringComparison.Ordinal) - idxSv));

                if (newSpreadsheetVersion.Length > 19) // Check for corruption
                    newSpreadsheetVersion = string.Empty;

                if (scriptVersion != LocalizationManager.GetRequiredWebServiceVersion())
                {
                    return "The current Google WebService is not supported.\nPlease, delete the WebService from the Google Drive and Install the latest version.";
                }

                //Debug.Log (Google_LastUpdatedVersion + " - " + newSpreadsheetVersion);
                if (saveInPlayerPrefs && !IsNewerVersion(googleLastUpdatedVersion, newSpreadsheetVersion))
#if UNITY_EDITOR
                    return "";
#else
				return "LanguageSource is up-to-date";
#endif

                if (saveInPlayerPrefs)
                {
                    string playerPrefName = GetSourcePlayerPrefName();
                    PersistentStorage.SaveFile(PersistentStorage.EFileType.Persistent, "I2Source_" + playerPrefName + ".loc", "[i2e]" + StringObfucator.Encode(jsonString));
                    PersistentStorage.SetSetting_String("I2SourceVersion_" + playerPrefName, newSpreadsheetVersion);
                    PersistentStorage.ForceSaveSettings();
                }
                googleLastUpdatedVersion = newSpreadsheetVersion;

                if (updateMode == ESpreadsheetUpdateMode.Replace)
                    ClearAllData();

                int csVstartIdx = jsonString.IndexOf("[i2category]", StringComparison.Ordinal);
                while (csVstartIdx > 0)
                {
                    csVstartIdx += "[i2category]".Length;
                    int endCat = jsonString.IndexOf("[/i2category]", csVstartIdx, StringComparison.Ordinal);
                    string category = jsonString.Substring(csVstartIdx, endCat - csVstartIdx);
                    endCat += "[/i2category]".Length;

                    int endCsv = jsonString.IndexOf("[/i2csv]", endCat, StringComparison.Ordinal);
                    string csv = jsonString.Substring(endCat, endCsv - endCat);

                    csVstartIdx = jsonString.IndexOf("[i2category]", endCsv, StringComparison.Ordinal);

                    Import_I2CSV(category, csv, updateMode);

                    // Only the first CSV should clear the Data
                    if (updateMode == ESpreadsheetUpdateMode.Replace)
                        updateMode = ESpreadsheetUpdateMode.Merge;
                }

                googleLiveSyncIsUptoDate = true;
                if (I2Utils.IsPlaying())
                {
                    SaveLanguages(true);
                }

                if (!string.IsNullOrEmpty(errorMsg))
                    Editor_SetDirty();
                return errorMsg;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                return e.ToString();
            }
		}

#endregion
	}
}