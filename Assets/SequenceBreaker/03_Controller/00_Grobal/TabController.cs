using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
sealed public class TabController : MonoBehaviour
{
    public Button timelineButton;
    public Button playButton;
    public Button homeButton;

    public GameObject timelineTab;
    public GameObject playTab;
    public GameObject homeTab;

    // Battle log generate 2019/9/2 for test
    //public GameObject BattleLogEnhancedScrollController;

    // Log list update

    public void ActivateTab(string toActivateTab)
    {
        switch (toActivateTab)
        {
            case "TimelineTab":
                timelineTab.SetActive(true);
                playTab.SetActive(false);
                homeTab.SetActive(false);


                //BattleLogEnhancedScrollController.GetComponent<BattleLogEnhancedScrollController>().DrawBattleLog();

                break;
            case "PlayTab":
                playTab.SetActive(true);
                homeTab.SetActive(false);
                timelineTab.SetActive(false);
                break;
            case "HomeTab":
                homeTab.SetActive(true);
                playTab.SetActive(false);
                timelineTab.SetActive(false);
                break;
            default:
                break;
        }
    }
}