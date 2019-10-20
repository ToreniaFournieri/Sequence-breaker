using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;


namespace I2.Loc
{
    using TranslationDictionary = Dictionary<string, TranslationQuery>;

    public class TranslationJobMain : TranslationJob
    {
        TranslationJobWeb _mWeb;
        TranslationJobPost _mPost;
        TranslationJobGet _mGet;

        TranslationDictionary _requests;
        GoogleTranslation.FnOnTranslationReady _onTranslationReady;
        public string MErrorMessage;

        public TranslationJobMain(TranslationDictionary requests, GoogleTranslation.FnOnTranslationReady onTranslationReady)
        {
            _requests = requests;
            _onTranslationReady = onTranslationReady;

            //mWeb = new TranslationJob_WEB(requests, OnTranslationReady);
            _mPost = new TranslationJobPost(requests, onTranslationReady);
        }

        public override EJobState GetState()
        {
            if (_mWeb != null)
            {
                var state = _mWeb.GetState();
                switch (state)
                {
                    case EJobState.Running: return EJobState.Running;
                    case EJobState.Succeeded:
                        {
                            MJobState = EJobState.Succeeded;
                            break;
                        }
                    case EJobState.Failed:
                        {
                            _mWeb.Dispose();
                            _mWeb = null;
                            _mPost = new TranslationJobPost(_requests, _onTranslationReady);
                            break;
                        }
                }
            }
            if (_mPost != null)
            {
                var state = _mPost.GetState();
                switch (state)
                {
                    case EJobState.Running: return EJobState.Running;
                    case EJobState.Succeeded:
                        {
                            MJobState = EJobState.Succeeded;
                            break;
                        }
                    case EJobState.Failed:
                        {
                            _mPost.Dispose();
                            _mPost = null;
                            _mGet = new TranslationJobGet(_requests, _onTranslationReady);
                            break;
                        }
                }
            }
            if (_mGet != null)
            {
                var state = _mGet.GetState();
                switch (state)
                {
                    case EJobState.Running: return EJobState.Running;
                    case EJobState.Succeeded:
                        {
                            MJobState = EJobState.Succeeded;
                            break;
                        }
                    case EJobState.Failed:
                        {
                            MErrorMessage = _mGet.MErrorMessage;
                            if (_onTranslationReady != null)
                                _onTranslationReady(_requests, MErrorMessage);
                            _mGet.Dispose();
                            _mGet = null;
                            break;
                        }
                }
            }

            return MJobState;
        }

        public override void Dispose()
        {
            if (_mPost != null) _mPost.Dispose();
            if (_mGet != null) _mGet.Dispose();
            _mPost = null;
            _mGet = null;
        }
    }
}