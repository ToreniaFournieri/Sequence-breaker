using UnityEngine;

namespace I2.Loc
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityStandardAudioSource : LocalizeTarget<AudioSource>
    {
        static LocalizeTargetUnityStandardAudioSource() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<AudioSource, LocalizeTargetUnityStandardAudioSource>() { Name = "AudioSource", Priority = 100 }); }

        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.AudioClip; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Text; }
        public override bool CanUseSecondaryTerm() { return false; }
        public override bool AllowMainTermToBeRtl() { return false; }
        public override bool AllowSecondTermToBeRtl() { return false; }

        public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = mTarget.clip ? mTarget.clip.name : string.Empty;
            secondaryTerm = null;
        }


        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            bool bIsPlaying = (mTarget.isPlaying || mTarget.loop) && Application.isPlaying;
            AudioClip oldClip = mTarget.clip;
            AudioClip newClip = cmp.FindTranslatedObject<AudioClip>(mainTranslation);
            if (oldClip != newClip)
                mTarget.clip = newClip;

            if (bIsPlaying && mTarget.clip)
                mTarget.Play();

            // If the old clip is not in the translatedObjects, then unload it as it most likely was loaded from Resources
            //if (!HasTranslatedObject(OldClip))
            //	Resources.UnloadAsset(OldClip);
        }
    }
}
