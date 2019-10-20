using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;

#pragma warning disable 618

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityStandardSpriteRenderer : LocalizeTarget<SpriteRenderer>
    {
        static LocalizeTargetUnityStandardSpriteRenderer() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<SpriteRenderer, LocalizeTargetUnityStandardSpriteRenderer>() { Name = "SpriteRenderer", Priority = 100 }); }

        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.Sprite; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Text; }
        public override bool CanUseSecondaryTerm() { return false; }
        public override bool AllowMainTermToBeRtl() { return false; }
        public override bool AllowSecondTermToBeRtl() { return false; }

        public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = mTarget.sprite != null ? mTarget.sprite.name : string.Empty;
            secondaryTerm = null;
        }

        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            Sprite old = mTarget.sprite;
            if (old == null || old.name != mainTranslation)
                mTarget.sprite = cmp.FindTranslatedObject<Sprite>(mainTranslation);

            // If the old value is not in the translatedObjects, then unload it as it most likely was loaded from Resources
            //if (!HasTranslatedObject(Old))
            //	Resources.UnloadAsset(Old);
        }
    }
}
