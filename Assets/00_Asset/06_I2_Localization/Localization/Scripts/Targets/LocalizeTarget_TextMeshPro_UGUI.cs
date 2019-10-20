using System;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

#if TextMeshPro
namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetTextMeshProUgui : LocalizeTarget<TMPro.TextMeshProUGUI>
    {
        static LocalizeTargetTextMeshProUgui() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<TMPro.TextMeshProUGUI, LocalizeTargetTextMeshProUgui>() { Name = "TextMeshPro UGUI", Priority = 100 }); }

        [FormerlySerializedAs("mAlignment_RTL")] public TMPro.TextAlignmentOptions mAlignmentRtl = TMPro.TextAlignmentOptions.Right;
        [FormerlySerializedAs("mAlignment_LTR")] public TMPro.TextAlignmentOptions mAlignmentLtr = TMPro.TextAlignmentOptions.Left;
        [FormerlySerializedAs("mAlignmentWasRTL")] public bool mAlignmentWasRtl;
        public bool mInitializeAlignment = true;

        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.Text; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.TextMeshPFont; }
        public override bool CanUseSecondaryTerm() { return true; }
        public override bool AllowMainTermToBeRtl() { return true; }
        public override bool AllowSecondTermToBeRtl() { return false; }

        public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = mTarget ? mTarget.text : null;
            secondaryTerm = (mTarget.font != null ? mTarget.font.name : string.Empty);
        }



        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            {
                //--[ Localize Font Object ]----------
                TMPro.TMP_FontAsset newFont = cmp.GetSecondaryTranslatedObj<TMPro.TMP_FontAsset>(ref mainTranslation, ref secondaryTranslation);

                if (newFont != null)
                {
                    LocalizeTargetTextMeshProLabel.SetFont(mTarget, newFont);
                }
                else
                {
                    //--[ Localize Font Material ]----------
                    Material newMat = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
                    if (newMat != null && mTarget.fontMaterial != newMat)
                    {
                        if (!newMat.name.StartsWith(mTarget.font.name, StringComparison.Ordinal))
                        {
                            newFont = LocalizeTargetTextMeshProLabel.GetTmpFontFromMaterial(cmp, secondaryTranslation.EndsWith(newMat.name, StringComparison.Ordinal) ? secondaryTranslation : newMat.name);
                            if (newFont != null)
                                LocalizeTargetTextMeshProLabel.SetFont(mTarget, newFont);
                        }
                        LocalizeTargetTextMeshProLabel.SetMaterial( mTarget, newMat );
                    }
                }
            }

            if (mInitializeAlignment)
            {
                mInitializeAlignment = false;
                mAlignmentWasRtl = LocalizationManager.IsRight2Left;
                LocalizeTargetTextMeshProLabel.InitAlignment_TMPro(mAlignmentWasRtl, mTarget.alignment, out mAlignmentLtr, out mAlignmentRtl);
            }
            else
            {
                TMPro.TextAlignmentOptions alignRtl, alignLtr;
                LocalizeTargetTextMeshProLabel.InitAlignment_TMPro(mAlignmentWasRtl, mTarget.alignment, out alignLtr, out alignRtl);

                if ((mAlignmentWasRtl && mAlignmentRtl != alignRtl) ||
                    (!mAlignmentWasRtl && mAlignmentLtr != alignLtr))
                {
                    mAlignmentLtr = alignLtr;
                    mAlignmentRtl = alignRtl;
                }
                mAlignmentWasRtl = LocalizationManager.IsRight2Left;
            }

            if (mainTranslation != null && mTarget.text != mainTranslation)
            {
                if (mainTranslation != null && cmp.correctAlignmentForRtl)
                {
                    mTarget.alignment = (LocalizationManager.IsRight2Left ? mAlignmentRtl : mAlignmentLtr);
                    mTarget.isRightToLeftText = LocalizationManager.IsRight2Left;
                    if (LocalizationManager.IsRight2Left) mainTranslation = I2Utils.ReverseText(mainTranslation);
                }

                mTarget.text = mainTranslation;
            }
        }
    }
}
#endif