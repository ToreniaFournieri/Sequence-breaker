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

    public class TranslationJobWeb : TranslationJobWww
    {
        TranslationDictionary _requests;
        GoogleTranslation.FnOnTranslationReady _onTranslationReady;
        public string MErrorMessage;

        string _mCurrentBatchToLanguageCode;
        string _mCurrentBatchFromLanguageCode;
        List<string> _mCurrentBatchText;

        List<KeyValuePair<string, string>> _mQueries;

        public TranslationJobWeb(TranslationDictionary requests, GoogleTranslation.FnOnTranslationReady onTranslationReady)
        {
            _requests = requests;
            _onTranslationReady = onTranslationReady;

            FindAllQueries();
            ExecuteNextBatch();
        }

        void FindAllQueries()
        {
            _mQueries = new List<KeyValuePair<string, string>>();
            foreach (var kvp in _requests)
            {
                foreach (var langCode in kvp.Value.TargetLanguagesCode)
                {
                    _mQueries.Add(new KeyValuePair<string, string>(kvp.Value.OrigText, kvp.Value.LanguageCode+":"+langCode));
                }
            }

            _mQueries.Sort((a, b) => a.Value.CompareTo(b.Value));
        }

        void ExecuteNextBatch()
        {
            if (_mQueries.Count==0)
            {
                MJobState = EJobState.Succeeded;
                return;
            }
            _mCurrentBatchText = new List<string>();

            string lastLangCode = null;
            int maxLength = 200;

            var sb = new StringBuilder();
            int i;
            for (i=0; i<_mQueries.Count; ++i)
            {
                var text = _mQueries[i].Key;
                var langCode = _mQueries[i].Value;

                if (lastLangCode==null || langCode==lastLangCode)
                {
                    if (i != 0)
                        sb.Append("|||");
                    sb.Append(text);

                    _mCurrentBatchText.Add(text);
                    lastLangCode = langCode;
                }
                if (sb.Length > maxLength)
                    break;
            }
            _mQueries.RemoveRange(0, i);

            var fromtoLang = lastLangCode.Split(':');
            _mCurrentBatchFromLanguageCode = fromtoLang[0];
            _mCurrentBatchToLanguageCode = fromtoLang[1];

            string url = string.Format ("http://www.google.com/translate_t?hl=en&vi=c&ie=UTF8&oe=UTF8&submit=Translate&langpair={0}|{1}&text={2}", _mCurrentBatchFromLanguageCode, _mCurrentBatchToLanguageCode, Uri.EscapeUriString( sb.ToString() ));
            Debug.Log(url);

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

            if (Www == null)
            {
                ExecuteNextBatch();
            }

            return MJobState;
        }

        public void ProcessResult(byte[] bytes, string errorMsg)
        {
            if (string.IsNullOrEmpty(errorMsg))
            {
                var wwwText = Encoding.UTF8.GetString(bytes, 0, bytes.Length); //www.text
                var result = ParseTranslationResult(wwwText, "aab");
                //errorMsg = GoogleTranslation.ParseTranslationResult(wwwText, _requests);
                Debug.Log(result);

                if (string.IsNullOrEmpty(errorMsg))
                {
                    if (_onTranslationReady != null)
                        _onTranslationReady(_requests, null);
                    return;
                }
            }
            
            MJobState = EJobState.Failed;
            MErrorMessage = errorMsg;
        }

        string ParseTranslationResult( string html, string originalText )
        {
            try
            {
                // This is a Hack for reading Google Translation while Google doens't change their response format
                int iStart = html.IndexOf("TRANSLATED_TEXT='") + "TRANSLATED_TEXT='".Length;
                int iEnd = html.IndexOf("';var", iStart);

                string translation = html.Substring( iStart, iEnd-iStart);

                // Convert to normalized HTML
                translation = System.Text.RegularExpressions.Regex.Replace(translation,
                                                                            @"\\x([a-fA-F0-9]{2})",
                                                                            match => char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)));

                // Convert ASCII Characters
                translation = System.Text.RegularExpressions.Regex.Replace(translation,
                                                                            @"&#(\d+);",
                                                                            match => char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value)));

                translation = translation.Replace("<br>", "\n");

                if (originalText.ToUpper()==originalText)
                    translation = translation.ToUpper();
                else
                    if (GoogleTranslation.UppercaseFirst(originalText)==originalText)
                        translation = GoogleTranslation.UppercaseFirst(translation);
                else
                    if (GoogleTranslation.TitleCase(originalText)==originalText)
                        translation = GoogleTranslation.TitleCase(translation);

                return translation;
            }
            catch (System.Exception ex) 
            { 
                Debug.LogError(ex.Message); 
                return string.Empty;
            }
        }
    }

 }