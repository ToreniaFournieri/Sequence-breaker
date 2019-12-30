using _00_Asset.I2.Localization.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace _00_Asset.I2.Localization.Scripts.Editor.Inspectors
{
	[CustomEditor(typeof(ResourceManager))]
	public class ResourceManagerInspector : UnityEditor.Editor 
	{
		SerializedProperty mAssets;

		void OnEnable()
		{
			UpgradeManager.EnablePlugins();
			mAssets = serializedObject.FindProperty("Assets");
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Space(5);
			GUITools.DrawHeader("Assets:", true);
			GUITools.BeginContents();
				///GUILayout.Label ("Assets:");
				GUITools.DrawObjectsArray( mAssets, false, false, false );
			GUITools.EndContents();

			serializedObject.ApplyModifiedProperties();
		}
	}
}