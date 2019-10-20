using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos.Main_Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void SceneButton_OnClick(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}