using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityUiText : LocalizeTarget<UnityEngine.UI.Text>
	{
        static LocalizeTargetUnityUiText() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<Text, LocalizeTargetUnityUiText>() { Name = "Text", Priority = 100 }); }

        TextAnchor _mAlignmentRtl = TextAnchor.UpperRight;
		TextAnchor _mAlignmentLtr = TextAnchor.UpperLeft;
		bool _mAlignmentWasRtl;
		bool _mInitializeAlignment = true;

        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.Text; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Font; }
        public override bool CanUseSecondaryTerm ()		{ return true;   }
		public override bool AllowMainTermToBeRtl ()	{ return true;   }
		public override bool AllowSecondTermToBeRtl ()	{ return false;  }

		public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm )
		{
            primaryTerm = mTarget ? mTarget.text : null;
            secondaryTerm = (mTarget.font!=null ? mTarget.font.name : string.Empty); ;
		}


		public override void DoLocalize ( Localize cmp, string mainTranslation, string secondaryTranslation )
		{
            //--[ Localize Font Object ]----------
            Font newFont = cmp.GetSecondaryTranslatedObj<Font>( ref mainTranslation, ref secondaryTranslation );
			if (newFont!=null && newFont!=mTarget.font)
				mTarget.font = newFont;

			if (_mInitializeAlignment)
			{
				_mInitializeAlignment = false;
				_mAlignmentWasRtl = LocalizationManager.IsRight2Left;
				InitAlignment( _mAlignmentWasRtl, mTarget.alignment, out _mAlignmentLtr, out _mAlignmentRtl );
			}
			else
			{
				TextAnchor alignRtl, alignLtr;
				InitAlignment( _mAlignmentWasRtl, mTarget.alignment, out alignLtr, out alignRtl );

				if ((_mAlignmentWasRtl && _mAlignmentRtl!=alignRtl) ||
					(!_mAlignmentWasRtl && _mAlignmentLtr != alignLtr))
				{
					_mAlignmentLtr = alignLtr;
					_mAlignmentRtl = alignRtl;
				}
				_mAlignmentWasRtl = LocalizationManager.IsRight2Left;
			}

			if (mainTranslation!=null && mTarget.text != mainTranslation)
			{
				if (cmp.correctAlignmentForRtl)
				{
					mTarget.alignment = LocalizationManager.IsRight2Left ? _mAlignmentRtl : _mAlignmentLtr;
				}


				mTarget.text = mainTranslation;
				mTarget.SetVerticesDirty();

				// In the editor, sometimes unity "forgets" to show the changes
                #if UNITY_EDITOR
				if (!Application.isPlaying)
					UnityEditor.EditorUtility.SetDirty( mTarget );
                #endif
			}
		}

		void InitAlignment ( bool isRtl, TextAnchor alignment, out TextAnchor alignLtr, out TextAnchor alignRtl )
		{
			alignLtr = alignRtl = alignment;

			if (isRtl)
			{
				switch (alignment)
				{
					case TextAnchor.UpperRight: alignLtr = TextAnchor.UpperLeft; break;
					case TextAnchor.MiddleRight: alignLtr = TextAnchor.MiddleLeft; break;
					case TextAnchor.LowerRight: alignLtr = TextAnchor.LowerLeft; break;
					case TextAnchor.UpperLeft: alignLtr = TextAnchor.UpperRight; break;
					case TextAnchor.MiddleLeft: alignLtr = TextAnchor.MiddleRight; break;
					case TextAnchor.LowerLeft: alignLtr = TextAnchor.LowerRight; break;
				}
			}
			else
			{
				switch (alignment)
				{
					case TextAnchor.UpperRight: alignRtl = TextAnchor.UpperLeft; break;
					case TextAnchor.MiddleRight: alignRtl = TextAnchor.MiddleLeft; break;
					case TextAnchor.LowerRight: alignRtl = TextAnchor.LowerLeft; break;
					case TextAnchor.UpperLeft: alignRtl = TextAnchor.UpperRight; break;
					case TextAnchor.MiddleLeft: alignRtl = TextAnchor.MiddleRight; break;
					case TextAnchor.LowerLeft: alignRtl = TextAnchor.LowerRight; break;
				}
			}
		}
	}
}

