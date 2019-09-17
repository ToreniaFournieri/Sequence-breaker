using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic;
using frame8.ScrollRectItemsAdapter.Classic.Util;



public class LogClientViewsHolder<TClientModel> : CAbstractViewsHolder where TClientModel : LogBaseClientModel
{
    public LayoutElement layoutElement;
    public Image averageScoreFillImage;
    public Text nameText, locationText;
    //public Text averageScoreText;
    //public RectTransform levelOfMission;
    public Slider levelOfMissionSlider;
    public GameObject transparentMessageController; // to display transparent message

    //Show detail button objects
    public ShowDetailButton detailButton;

    // Log related values
    public KohmaiWorks.Scroller.BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value
    public GameObject battleLog;
    public GameObject logList;
    public GameObject battle;

    public Text levelOfMissionText;
    public Text resultText;

    // to decide the result
    public WhichWin whichWin;

    /// <inheritdoc/>
    public override void CollectViews()
    {
        base.CollectViews();

        layoutElement = root.GetComponent<LayoutElement>();

        var mainPanel = root.GetChild(0);
        resultText = mainPanel.Find("NameAndLocationPanel/Result").GetComponent<Text>();
        levelOfMissionText = mainPanel.Find("MissionImage/LevelText").GetComponent<Text>();
        nameText = mainPanel.Find("NameAndLocationPanel/MissionText").GetComponent<Text>();        
        locationText = mainPanel.Find("NameAndLocationPanel/LocationText").GetComponent<Text>();

        Transform showDetailPanel = root.GetChild(2);
        detailButton = showDetailPanel.GetComponent<ShowDetailButton>();

    }

    //public void SetBattle()
    //{
    //    //Set battle gameobject to activate GO button
    //    var go = root.GetChild(2);
    //    goScript = go.GetComponent<GoScript>();

    //    goScript.Battle = battle;
    //    //goScript.battleLogEnhancedScrollController = battleLogEnhancedScrollController;
    //    goScript.transparentMessage = transparentMessageController;
    //}

    public virtual void UpdateViews(TClientModel dataModel)
    {
        nameText.text = dataModel.missionName + "(#" + ItemIndex + ")";
        locationText.text = "  " + dataModel.location;

        //ShowDetailButton showDetailButton = detailButton.GetComponent<ShowDetailButton>();

        detailButton.battle = battle;
        detailButton.battleLogEnhancedScrollController = battleLogEnhancedScrollController;
        detailButton.battleLog = battleLog;
        detailButton.logList = logList;

        


        //Debug.Log("which win? :" + battle.GetComponent<RunBattle>().whichWin + " in " + battle.GetComponent<RunBattle>().missionText) ;

        //resultText.text = null;
        //switch (whichWin)
        //{
        //    case WhichWin.none:
        //        break;
        //    case WhichWin.allyWin:
        //        resultText.text = "[Win]";
        //        break;
        //    case WhichWin.enemyWin:
        //        resultText.text = "[Lose]";
        //        break;
        //    case WhichWin.Draw:
        //        resultText.text = "[Draw]";
        //        break;
        //    default:
        //        Debug.LogError("Unexpected value :" + battle.GetComponent<RunBattle>().whichWin);
        //        break;

        //}
        //UpdateScores(dataModel);
        //SetBattle();

    }

    //void UpdateScores(LogList1ClientModel dataModel)
    //{

    //    //levelOfMissionText.text = "lv: " + (int)levelOfMissionSlider.value ;

    //}
}

public class LogClientViewsHolder : LogClientViewsHolder<LogBaseClientModel>
{

}

//public static class CUtil
//{

//    // Utility randomness methods
//    public static int Rand(int maxExcl) { return UnityEngine.Random.Range(0, maxExcl); }
//    public static float RandF(float max = 1f) { return UnityEngine.Random.Range(0, max); }
//}

public class LogBaseClientModel
{
    public string missionName;
    public string location;
    public WhichWin whichWin;

    //Obsolate
    public float availability01, contractChance01, longTermClient01;
    public bool isOnline;

    public float AverageScore01 { get { return (availability01 + contractChance01 + longTermClient01) / 3; } }

    public void SetRandom()
    {
        availability01 = CUtil.RandF();
        contractChance01 = CUtil.RandF();
        longTermClient01 = CUtil.RandF();
        isOnline = CUtil.Rand(2) == 0;
    }
}


public class LogClientModel : LogBaseClientModel
{
    // View size related
    public bool expanded;
    public float nonExpandedSize;
}


public class LogViewsHolder : LogClientViewsHolder<LogClientModel>
{
    public CExpandCollapseOnClick expandCollapseComponent;


    public override void CollectViews()
    {
        base.CollectViews();

        expandCollapseComponent = root.GetComponent<CExpandCollapseOnClick>();
    }

    public override void UpdateViews(LogClientModel dataModel)
    {
        base.UpdateViews(dataModel);

        if (expandCollapseComponent)
        {
            expandCollapseComponent.expanded = dataModel.expanded;
            expandCollapseComponent.nonExpandedSize = dataModel.nonExpandedSize;
        }
    }
}

