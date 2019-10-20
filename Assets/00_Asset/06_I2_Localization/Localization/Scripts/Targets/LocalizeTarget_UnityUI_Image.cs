using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityUiImage : LocalizeTarget<UnityEngine.UI.Image>
	{
        static LocalizeTargetUnityUiImage() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<Image, LocalizeTargetUnityUiImage>() { Name = "Image", Priority = 100 }); }

		public override bool CanUseSecondaryTerm () { return false; }
		public override bool AllowMainTermToBeRtl () { return false; }
		public override bool AllowSecondTermToBeRtl () { return false; }
        public override ETermType GetPrimaryTermType(Localize cmp)
        {
            return mTarget.sprite == null ? ETermType.Texture : ETermType.Sprite;
        }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Text; }


        public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm )
		{
            primaryTerm = mTarget.mainTexture ? mTarget.mainTexture.name : "";
            if (mTarget.sprite!=null && mTarget.sprite.name!=primaryTerm)
                primaryTerm += "." + mTarget.sprite.name;

			secondaryTerm = null;
		}


		public override void DoLocalize ( Localize cmp, string mainTranslation, string secondaryTranslation )
		{
            Sprite old = mTarget.sprite;
			if (old==null || old.name!=mainTranslation)
				mTarget.sprite = cmp.FindTranslatedObject<Sprite>( mainTranslation );

			// If the old value is not in the translatedObjects, then unload it as it most likely was loaded from Resources
			//if (!HasTranslatedObject(Old))
			//	Resources.UnloadAsset(Old);

			// In the editor, sometimes unity "forgets" to show the changes
#if UNITY_EDITOR
			if (!Application.isPlaying)
				UnityEditor.EditorUtility.SetDirty( mTarget );
#endif
		}
	}
}
