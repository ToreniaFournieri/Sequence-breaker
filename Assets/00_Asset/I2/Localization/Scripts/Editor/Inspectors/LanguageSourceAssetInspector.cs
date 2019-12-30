using _00_Asset.I2.Localization.Scripts.LanguageSource;
using UnityEditor;

namespace _00_Asset.I2.Localization.Scripts.Editor.Inspectors
{
    [CustomEditor(typeof(LanguageSourceAsset))]
    public partial class LanguageSourceAssetInspector : Localization.LocalizationEditor
    {
        void OnEnable()
        {
            var newSource = target as LanguageSourceAsset;
            SerializedProperty propSource = serializedObject.FindProperty("mSource");

            Custom_OnEnable(newSource.mSource, propSource);
        }
        public override LanguageSourceData GetSourceData()
        {
            return (target as LanguageSourceAsset).mSource;
        }
    }
}