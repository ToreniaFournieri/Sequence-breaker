using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Google
{
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