using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace I2.Loc
{
    using TranslationDictionary = Dictionary<string, TranslationQuery>;


    public class TranslationJob : IDisposable
    {
        public EJobState MJobState = EJobState.Running;

        public enum EJobState { Running, Succeeded, Failed };

        public virtual EJobState GetState() { return MJobState; }

        public virtual void Dispose() { }

    }

    public class TranslationJobWww : TranslationJob
    {
        public UnityWebRequest Www;

        public override void Dispose()
        {
            if (Www!=null)
                Www.Dispose();
            Www = null;
        }

    }
}