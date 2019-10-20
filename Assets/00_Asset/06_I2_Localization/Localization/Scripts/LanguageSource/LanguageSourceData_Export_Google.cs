using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace I2.Loc
{
	public enum ESpreadsheetUpdateMode { None, Replace, Merge, AddNewTerms };

	public partial class LanguageSourceData
	{
		public UnityWebRequest Export_Google_CreateWWWcall( ESpreadsheetUpdateMode updateMode = ESpreadsheetUpdateMode.Replace )
		{
            #if UNITY_WEBPLAYER
			Debug.Log ("Contacting google translation is not yet supported on WebPlayer" );
			return null;
#else
            string data = Export_Google_CreateData();

			WWWForm form = new WWWForm();
			form.AddField("key", googleSpreadsheetKey);
			form.AddField("action", "SetLanguageSource");
			form.AddField("data", data);
			form.AddField("updateMode", updateMode.ToString());

            #if UNITY_EDITOR
            form.AddField("password", googlePassword);
#endif


            UnityWebRequest www = UnityWebRequest.Post(LocalizationManager.GetWebServiceUrl(this), form);
            I2Utils.SendWebRequest(www);
            return www;
			#endif
		}

		string Export_Google_CreateData()
		{
			List<string> categories = GetCategories(true);
			StringBuilder builder = new StringBuilder();
			
			bool bFirst = true;
			foreach (string category in categories)
			{
				if (bFirst)
					bFirst = false;
				else
					builder.Append("<I2Loc>");

                #if !UNITY_EDITOR
                    bool Spreadsheet_SpecializationAsRows = true;
                #endif

                string csv = Export_I2CSV(category, specializationsAsRows:spreadsheetSpecializationAsRows);
				builder.Append(category);
				builder.Append("<I2Loc>");
				builder.Append(csv);
			}
			return builder.ToString();
		}
	}
}