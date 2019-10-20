using System;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine.Serialization;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{
    [Serializable]
    public struct LocalizedString
    {
        public string mTerm;
        [FormerlySerializedAs("mRTL_IgnoreArabicFix")] public bool mRtlIgnoreArabicFix;
        [FormerlySerializedAs("mRTL_MaxLineLength")] public int  mRtlMaxLineLength;
        [FormerlySerializedAs("mRTL_ConvertNumbers")] public bool mRtlConvertNumbers;
        [FormerlySerializedAs("m_DontLocalizeParameters")] public bool mDontLocalizeParameters;

        public static implicit operator string(LocalizedString s)
        {
            return s.ToString();
        }

        public static implicit operator LocalizedString(string term)
        {
            return new LocalizedString() { mTerm = term };
        }

        public LocalizedString (LocalizedString str)
        {
            mTerm = str.mTerm;
            mRtlIgnoreArabicFix = str.mRtlIgnoreArabicFix;
            mRtlMaxLineLength = str.mRtlMaxLineLength;
            mRtlConvertNumbers = str.mRtlConvertNumbers;
            mDontLocalizeParameters = str.mDontLocalizeParameters;
        }



        public override string ToString()
        {
            var translation = LocalizationManager.GetTranslation(mTerm, !mRtlIgnoreArabicFix, mRtlMaxLineLength, !mRtlConvertNumbers, true );
            LocalizationManager.ApplyLocalizationParams(ref translation, !mDontLocalizeParameters);
            return translation;
        }
    }
}