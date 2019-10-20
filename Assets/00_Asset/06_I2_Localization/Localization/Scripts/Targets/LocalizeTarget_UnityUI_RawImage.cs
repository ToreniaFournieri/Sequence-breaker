using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityUiRawImage : LocalizeTarget<RawImage>
    {
        static LocalizeTargetUnityUiRawImage() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<RawImage, LocalizeTargetUnityUiRawImage>() { Name = "RawImage", Priority = 100 }); }

        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.Texture; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Text; }
        public override bool CanUseSecondaryTerm() { return false; }
        public override bool AllowMainTermToBeRtl() { return false; }
        public override bool AllowSecondTermToBeRtl() { return false; }


        public override void GetFinalTerms(Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = mTarget.mainTexture ? mTarget.mainTexture.name : "";
            secondaryTerm = null;
        }


        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            Texture old = mTarget.texture;
            if (old == null || old.name != mainTranslation)
                mTarget.texture = cmp.FindTranslatedObject<Texture>(mainTranslation);

            // If the old value is not in the translatedObjects, then unload it as it most likely was loaded from Resources
            //if (!HasTranslatedObject(Old))
            //	Resources.UnloadAsset(Old);

            // In the editor, sometimes unity "forgets" to show the changes
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(mTarget);
            #endif
        }
    }
}