using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabController : MonoBehaviour
{
    public Button timelineButton;
    public Button playButton;
    public Button homeButton;

    public GameObject timelineTab;
    public GameObject playTab;
    public GameObject homeTab;

    public void ActivateTab(string toActivateTab)
    {
        switch (toActivateTab)
        {
            case "TimelineTab":
                timelineTab.SetActive(true);
                playTab.SetActive(false);
                homeTab.SetActive(false);
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