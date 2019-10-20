using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos.Main_Menu
{
    public class ReturnToMainMenu : MonoBehaviour
    {
        public void ReturnToMainMenuButton_OnClick()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}