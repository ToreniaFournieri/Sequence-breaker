using UnityEngine;
using System.Collections;

namespace SuperScrollView
{

    public class FpsDisplay : MonoBehaviour
    {
        float _deltaTime = 0.0f;

        GUIStyle _mStyle;
        void Awake()
        {
            _mStyle = new GUIStyle();
            _mStyle.alignment = TextAnchor.UpperLeft;
            _mStyle.normal.background = null;
            _mStyle.fontSize = 25;
            _mStyle.normal.textColor = new Color(0f, 1f, 0f, 1.0f);
        }

        void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;
            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            float fps = 1.0f / _deltaTime;
            string text = string.Format("   {0:0.} FPS", fps);
            GUI.Label(rect, text, _mStyle);
        }
    }
}
