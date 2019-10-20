using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;

#pragma warning disable 618

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityStandardTextMesh : LocalizeTarget<TextMesh>
    {
        static LocalizeTargetUnityStandardTextMesh() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<TextMesh, LocalizeTargetUnityStandardTextMesh>() { Name = "TextMesh", Priority = 100 }); }

        TextAlignment _mAlignmentRtl = TextAlignment.Right;
        TextAlignment _mAlignmentLtr = TextAlignment.Left;
        bool _mAlignmentWasRtl;
        bool _mInitializeAlignment = true;

        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.Text; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Font; }
        public override bool CanUseSecondaryTerm() { return true; }
        public override bool AllowMainTermToBeRtl() { return true; }
        public override bool AllowSecondTermToBeRtl() { return false; }

        public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = mTarget ? mTarget.text : null;
            secondaryTerm = (string.IsNullOrEmpty(secondary) && mTarget.font != null) ? mTarget.font.name : null;
        }

        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            //--[ Localize Font Object ]----------
            Font newFont = cmp.GetSecondaryTranslatedObj<Font>(ref mainTranslation, ref secondaryTranslation);
            if (newFont != null && mTarget.font != newFont)
                mTarget.font = newFont;

            //--[ Localize Text ]----------
            if (_mInitializeAlignment)
            {
                _mInitializeAlignment = false;

                _mAlignmentLtr = _mAlignmentRtl = mTarget.alignment;

                if (LocalizationManager.IsRight2Left && _mAlignmentRtl == TextAlignment.Right)
                    _mAlignmentLtr = TextAlignment.Left;
                if (!LocalizationManager.IsRight2Left && _mAlignmentLtr == TextAlignment.Left)
                    _mAlignmentRtl = TextAlignment.Right;

            }
            if (mainTranslation != null && mTarget.text != mainTranslation)
            {
                if (cmp.correctAlignmentForRtl && mTarget.alignment != TextAlignment.Center)
                    mTarget.alignment = (LocalizationManager.IsRight2Left ? _mAlignmentRtl : _mAlignmentLtr);

                mTarget.font.RequestCharactersInTexture(mainTranslation);
                mTarget.text = mainTranslation;
            }
        }
    }
}