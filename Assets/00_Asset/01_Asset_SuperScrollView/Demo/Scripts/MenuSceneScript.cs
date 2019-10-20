using _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts
{
    public class SceneNameInfo
    {
        public string MName;
        public string MSceneName;
        public SceneNameInfo(string name,string sceneName)
        {
            MName = name;
            MSceneName = sceneName;
        }
    }
    class MenuSceneScript: MonoBehaviour
    {
        public Transform mButtonPanelTf;
        SceneNameInfo[] _mSceneNameArray = new SceneNameInfo[]
        {
            new SceneNameInfo("Staggered GridView1","StaggeredGridView_TopToBottom"),
            new SceneNameInfo("Staggered GridView2","StaggeredGridView_LeftToRight"),
            new SceneNameInfo("Chat Message List","ChatMsgListViewDemo"),
            new SceneNameInfo("Horizontal Gallery","HorizontalGalleryDemo"),
            new SceneNameInfo("Vertical Gallery","VerticalGalleryDemo"),
            new SceneNameInfo("GridView","GridView_TopLeftToBottomRight"),
            new SceneNameInfo("PageView","PageViewDemo"),
            new SceneNameInfo("TreeView","TreeViewDemo"),
            new SceneNameInfo("Spin Date Picker","SpinDatePickerDemo"),
            new SceneNameInfo("Pull Down To Refresh","PullAndRefreshDemo"),
            new SceneNameInfo("TreeView\nWith Sticky Head","TreeViewWithStickyHeadDemo"),
            new SceneNameInfo("Change Item Height","ChangeItemHeightDemo"),
            new SceneNameInfo("Pull Up To Load More","PullAndLoadMoreDemo"),
            new SceneNameInfo("Click Load More","ClickAndLoadMoreDemo"),
            new SceneNameInfo("Select And Delete","DeleteItemDemo"),
            new SceneNameInfo("GridView Select Delete ","GridViewDeleteItemDemo"),
            new SceneNameInfo("Responsive GridView","ResponsiveGridViewDemo"),
            new SceneNameInfo("TreeView\nWith Children Indent","TreeViewWithChildrenIndentDemo"),

        };
        void Start()
        {
            CreateFpsDisplyObj();
            int count = mButtonPanelTf.childCount;
            int sceneCount = _mSceneNameArray.Length;
            for (int i = 0;i< count;++i)
            {
                if(i >= sceneCount)
                {
                    mButtonPanelTf.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                mButtonPanelTf.GetChild(i).gameObject.SetActive(true);
                SceneNameInfo info = _mSceneNameArray[i];
                Button button = mButtonPanelTf.GetChild(i).GetComponent<Button>();
                button.onClick.AddListener(delegate ()
                {
                    SceneManager.LoadScene(info.MSceneName);
                });
                Text text = button.transform.Find("Text").GetComponent<Text>();
                text.text = info.MName;
            }

        }

        void CreateFpsDisplyObj()
        {
            FpsDisplay fpsObj = FindObjectOfType<FpsDisplay>();
            if(fpsObj != null)
            {
                return;
            }
            GameObject go = new GameObject();
            go.name = "FPSDisplay";
            go.AddComponent<FpsDisplay>();
            DontDestroyOnLoad(go);
        }

    }
}
