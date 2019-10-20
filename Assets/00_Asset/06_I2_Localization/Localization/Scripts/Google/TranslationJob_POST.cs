using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.Networking;

namespace I2.Loc
{
    using TranslationDictionary = Dictionary<string, TranslationQuery>;

    public class TranslationJobPost : TranslationJobWww
    {
        TranslationDictionary _requests;
        GoogleTranslation.FnOnTranslationReady _onTranslationReady;

        public TranslationJobPost(TranslationDictionary requests, GoogleTranslation.FnOnTranslationReady onTranslationReady)
        {
            _requests = requests;
            _onTranslationReady = onTranslationReady;

            var data = GoogleTranslation.ConvertTranslationRequest(requests, false);

            WWWForm form = new WWWForm();
            form.AddField("action", "Translate");
            form.AddField("list", data[0]);

            Www = UnityWebRequest.Post(LocalizationManager.GetWebServiceUrl(), form);
            I2Utils.SendWebRequest(Www);
        }

        public override EJobState GetState()
        {
            if (Www != null && Www.isDone)
            {
                ProcessResult(Www.downloadHandler.data, Www.error);
                Www.Dispose();
                Www = null;
            }

            return MJobState;
        }

        public void ProcessResult(byte[] bytes, string errorMsg)
        {
            if (!string.IsNullOrEmpty(errorMsg))
            {
                // check for 
                //if (errorMsg.Contains("rewind"))  // "necessary data rewind wasn't possible"
                MJobState = EJobState.Failed;                    
            }
            else
            {
                var wwwText = Encoding.UTF8.GetString(bytes, 0, bytes.Length); //www.text
                errorMsg = GoogleTranslation.ParseTranslationResult(wwwText, _requests);
                if (_onTranslationReady!=null)
                    _onTranslationReady(_requests, errorMsg);
                MJobState = EJobState.Succeeded;
            }
        }
    }
}