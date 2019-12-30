using UnityEngine;

namespace _00_Asset.I2.Localization.Scripts.LanguageSource
{
    [CreateAssetMenu(fileName = "I2Languages", menuName = "I2 Localization/LanguageSource", order = 1)]
    public class LanguageSourceAsset : ScriptableObject, ILanguageSource
    {
        public  LanguageSourceData SourceData
        {
            get { return mSource; }
            set { mSource = value; }
        }

        public LanguageSourceData mSource = new LanguageSourceData();
    }
}