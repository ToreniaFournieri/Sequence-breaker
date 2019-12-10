using _00_Asset.StompyRobot.SRDebugger.Scripts.Internal;
using _00_Asset.StompyRobot.SRDebugger.Scripts.Profiler;
using UnityEditor;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.Editor
{
    [CustomEditor(typeof (ProfilerCameraListener))]
    public class ProfilerCameraListenerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(SRDebugStrings.Current.ProfilerCameraListenerHelp, MessageType.Info, true);
        }
    }
}
