using System.Collections.Generic;
using System.Text;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;
using UnityEngine.Networking;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Google
{
    using TranslationDictionary = Dictionary<string, TranslationQuery>;

    public class TranslationJobGet : TranslationJobWww
    {
        TranslationDictionary _requests;
        GoogleTranslation.FnOnTranslationReady _onTranslationReady;
        List<string> _mQueries;
        public string MErrorMessage;

        public TranslationJobGet(TranslationDictionary requests, GoogleTranslation.FnOnTranslationReady onTranslationReady)
        {
            _requests = requests;
            _onTranslationReady = onTranslationReady;

            _mQueries = GoogleTranslation.ConvertTranslationRequest(requests, true);

            GetState();
        }

        void ExecuteNextQuery()
        {
            if (_mQueries.Count == 0)
            {
                MJobState = EJobState.Succeeded;
                return;
            }

            int lastQuery = _mQueries.Count - 1;
            var query = _mQueries[lastQuery];
            _mQueries.RemoveAt(lastQuery);

            string url = string.Format("{0}?action=Translate&list={1}", LocalizationManager.GetWebServiceUrl(), query);
            Www = UnityWebRequest.Get(url);
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

            if (Www==null)
            {
                ExecuteNextQuery();
            }

            return MJobState;
        }

        public void ProcessResult(byte[] bytes, string errorMsg)
        {
            if (string.IsNullOrEmpty(errorMsg))
            {
                var wwwText = Encoding.UTF8.GetString(bytes, 0, bytes.Length); //www.text
                errorMsg = GoogleTranslation.ParseTranslationResult(wwwText, _requests);

                if (string.IsNullOrEmpty(errorMsg))
                {
                    if (_onTranslationReady!=null)
                        _onTranslationReady(_requests, null);
                    return;
                }
            }

            MJobState = EJobState.Failed;
            MErrorMessage = errorMsg;
        }
    }
}