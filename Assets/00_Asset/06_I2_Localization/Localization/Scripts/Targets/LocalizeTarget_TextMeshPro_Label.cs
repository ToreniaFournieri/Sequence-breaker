using System;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;
using UnityEngine;

#if TextMeshPro
namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetTextMeshProLabel : LocalizeTarget<TMPro.TextMeshPro>
    {
        static LocalizeTargetTextMeshProLabel() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<TMPro.TextMeshPro, LocalizeTargetTextMeshProLabel>() { Name = "TextMeshPro Label", Priority = 100 }); }

        TMPro.TextAlignmentOptions _mAlignmentRtl = TMPro.TextAlignmentOptions.Right;
        TMPro.TextAlignmentOptions _mAlignmentLtr = TMPro.TextAlignmentOptions.Left;
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
            secondaryTerm = (mTarget.font != null ? mTarget.font.name : string.Empty);
        }

        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            //--[ Localize Font Object ]----------
            {
                TMPro.TMP_FontAsset newFont = cmp.GetSecondaryTranslatedObj<TMPro.TMP_FontAsset>(ref mainTranslation, ref secondaryTranslation);

                if (newFont != null)
                {
                    SetFont(mTarget, newFont);
                }
                else
                {
                    //--[ Localize Font Material ]----------
                    Material newMat = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
                    if (newMat != null && mTarget.fontMaterial != newMat)
                    {
                        if (!newMat.name.StartsWith(mTarget.font.name, StringComparison.Ordinal))
                        {
                            newFont = GetTmpFontFromMaterial(cmp, secondaryTranslation.EndsWith(newMat.name, StringComparison.Ordinal) ? secondaryTranslation : newMat.name);
                            if (newFont != null)
                                SetFont(mTarget, newFont);
                        }
                        SetMaterial(mTarget, newMat); 
                    }
                           
                }
            }
            if (_mInitializeAlignment)
            {
                _mInitializeAlignment = false;
                _mAlignmentWasRtl = LocalizationManager.IsRight2Left;
                InitAlignment_TMPro(_mAlignmentWasRtl, mTarget.alignment, out _mAlignmentLtr, out _mAlignmentRtl);
            }
            else
            {
                TMPro.TextAlignmentOptions alignRtl, alignLtr;
                InitAlignment_TMPro(_mAlignmentWasRtl, mTarget.alignment, out alignLtr, out alignRtl);

                if ((_mAlignmentWasRtl && _mAlignmentRtl != alignRtl) ||
                    (!_mAlignmentWasRtl && _mAlignmentLtr != alignLtr))
                {
                    _mAlignmentLtr = alignLtr;
                    _mAlignmentRtl = alignRtl;
                }
                _mAlignmentWasRtl = LocalizationManager.IsRight2Left;
            }

            if (mainTranslation != null && mTarget.text != mainTranslation)
            {
                if (mainTranslation != null && cmp.correctAlignmentForRtl)
                {
                    mTarget.alignment = (LocalizationManager.IsRight2Left ? _mAlignmentRtl : _mAlignmentLtr);
                    mTarget.isRightToLeftText = LocalizationManager.IsRight2Left;
                    if (LocalizationManager.IsRight2Left) mainTranslation = I2Utils.ReverseText(mainTranslation);
                }

                mTarget.text = mainTranslation;
            }
        }

        #region Tools
        internal static TMPro.TMP_FontAsset GetTmpFontFromMaterial(Localize cmp, string matName)
        {
            string splitChars = " .\\/-[]()";
            for (int i = matName.Length - 1; i > 0;)
            {
                // Find first valid character
                while (i > 0 && splitChars.IndexOf(matName[i]) >= 0)
                    i--;

                if (i <= 0) break;

                var fontName = matName.Substring(0, i + 1);
                var obj = cmp.GetObject<TMPro.TMP_FontAsset>(fontName);
                if (obj != null)
                    return obj;

                // skip this word
                while (i > 0 && splitChars.IndexOf(matName[i]) < 0)
                    i--;
            }

            return null;
        }

        internal static void InitAlignment_TMPro(bool isRtl, TMPro.TextAlignmentOptions alignment, out TMPro.TextAlignmentOptions alignLtr, out TMPro.TextAlignmentOptions alignRtl)
        {
            alignLtr = alignRtl = alignment;

            if (isRtl)
            {
                switch (alignment)
                {
                    case TMPro.TextAlignmentOptions.TopRight: alignLtr = TMPro.TextAlignmentOptions.TopLeft; break;
                    case TMPro.TextAlignmentOptions.Right: alignLtr = TMPro.TextAlignmentOptions.Left; break;
                    case TMPro.TextAlignmentOptions.BottomRight: alignLtr = TMPro.TextAlignmentOptions.BottomLeft; break;
                    case TMPro.TextAlignmentOptions.BaselineRight: alignLtr = TMPro.TextAlignmentOptions.BaselineLeft; break;
                    case TMPro.TextAlignmentOptions.MidlineRight: alignLtr = TMPro.TextAlignmentOptions.MidlineLeft; break;
                    case TMPro.TextAlignmentOptions.CaplineRight: alignLtr = TMPro.TextAlignmentOptions.CaplineLeft; break;

                    case TMPro.TextAlignmentOptions.TopLeft: alignLtr = TMPro.TextAlignmentOptions.TopRight; break;
                    case TMPro.TextAlignmentOptions.Left: alignLtr = TMPro.TextAlignmentOptions.Right; break;
                    case TMPro.TextAlignmentOptions.BottomLeft: alignLtr = TMPro.TextAlignmentOptions.BottomRight; break;
                    case TMPro.TextAlignmentOptions.BaselineLeft: alignLtr = TMPro.TextAlignmentOptions.BaselineRight; break;
                    case TMPro.TextAlignmentOptions.MidlineLeft: alignLtr = TMPro.TextAlignmentOptions.MidlineRight; break;
                    case TMPro.TextAlignmentOptions.CaplineLeft: alignLtr = TMPro.TextAlignmentOptions.CaplineRight; break;

                }
            }
            else
            {
                switch (alignment)
                {
                    case TMPro.TextAlignmentOptions.TopRight: alignRtl = TMPro.TextAlignmentOptions.TopLeft; break;
                    case TMPro.TextAlignmentOptions.Right: alignRtl = TMPro.TextAlignmentOptions.Left; break;
                    case TMPro.TextAlignmentOptions.BottomRight: alignRtl = TMPro.TextAlignmentOptions.BottomLeft; break;
                    case TMPro.TextAlignmentOptions.BaselineRight: alignRtl = TMPro.TextAlignmentOptions.BaselineLeft; break;
                    case TMPro.TextAlignmentOptions.MidlineRight: alignRtl = TMPro.TextAlignmentOptions.MidlineLeft; break;
                    case TMPro.TextAlignmentOptions.CaplineRight: alignRtl = TMPro.TextAlignmentOptions.CaplineLeft; break;

                    case TMPro.TextAlignmentOptions.TopLeft: alignRtl = TMPro.TextAlignmentOptions.TopRight; break;
                    case TMPro.TextAlignmentOptions.Left: alignRtl = TMPro.TextAlignmentOptions.Right; break;
                    case TMPro.TextAlignmentOptions.BottomLeft: alignRtl = TMPro.TextAlignmentOptions.BottomRight; break;
                    case TMPro.TextAlignmentOptions.BaselineLeft: alignRtl = TMPro.TextAlignmentOptions.BaselineRight; break;
                    case TMPro.TextAlignmentOptions.MidlineLeft: alignRtl = TMPro.TextAlignmentOptions.MidlineRight; break;
                    case TMPro.TextAlignmentOptions.CaplineLeft: alignRtl = TMPro.TextAlignmentOptions.CaplineRight; break;
                }
            }
        }

        internal static void SetFont(TMPro.TMP_Text label, TMPro.TMP_FontAsset newFont)
        {
            if (label.font != newFont)
            {
                label.font = newFont;
            }
            if (label.linkedTextComponent != null)
            {
                SetFont(label.linkedTextComponent, newFont);
            }
        }
        internal static void SetMaterial(TMPro.TMP_Text label, Material newMat)
        {
            if (label.fontSharedMaterial != newMat)
            {
                label.fontSharedMaterial = newMat;
            }
            if (label.linkedTextComponent != null)
            {
                SetMaterial(label.linkedTextComponent, newMat);
            }
        }
        #endregion
    }
}
#endif