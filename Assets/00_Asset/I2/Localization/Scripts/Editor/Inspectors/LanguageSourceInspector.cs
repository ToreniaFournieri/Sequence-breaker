using _00_Asset.I2.Localization.Scripts.LanguageSource;
using UnityEditor;

namespace _00_Asset.I2.Localization.Scripts.Editor.Inspectors
{
    [CustomEditor(typeof(LanguageSource.LanguageSource))]
    public partial class LanguageSourceInspector : Localization.LocalizationEditor
    {
        void OnEnable()
        {
            var newSource = target as LanguageSource.LanguageSource;
            SerializedProperty propSource = serializedObject.FindProperty("mSource");

            Custom_OnEnable(newSource.mSource, propSource);
        }

        public override LanguageSourceData GetSourceData()
        {
            return (target as LanguageSource.LanguageSource).mSource;
        }

    }
}