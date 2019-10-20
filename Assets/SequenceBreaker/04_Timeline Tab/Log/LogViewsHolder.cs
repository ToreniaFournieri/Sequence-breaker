using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic;
using frame8.ScrollRectItemsAdapter.Classic.Util;



public class LogClientViewsHolder<TClientModel> : CAbstractViewsHolder where TClientModel : LogBaseClientModel
{
    public LayoutElement LayoutElement;
    public Image AverageScoreFillImage;
    public Text NameText, LocationText;
    //public Text averageScoreText;
    //public RectTransform levelOfMission;
    public Slider LevelOfMissionSlider;
    public GameObject TransparentMessageController; // to display transparent message

    //Show detail button objects
    public ShowDetailButton DetailButton;

    // Log related values
    public KohmaiWorks.Scroller.BattleLogEnhancedScrollController BattleLogEnhancedScrollController; // to pass the log value
    public GameObject BattleLog;
    public GameObject LogList;
    public GameObject Battle;

    public Text LevelOfMissionText;
    public Text ResultText;

    // to decide the result
    public WhichWin WhichWin;

    /// <inheritdoc/>
    public override void CollectViews()
    {
        base.CollectViews();

        LayoutElement = Root.GetComponent<LayoutElement>();

        var mainPanel = Root.GetChild(0);
        ResultText = mainPanel.Find("NameAndLocationPanel/Result").GetComponent<Text>();

        LevelOfMissionText = mainPanel.Find("MissionImage/LevelText2").GetComponent<Text>();
        //levelOfMissionText = mainPanel.Find("MissionImage/LevelText").GetComponent<Text>();
        NameText = mainPanel.Find("NameAndLocationPanel/MissionText").GetComponent<Text>();
        LocationText = mainPanel.Find("NameAndLocationPanel/LocationText").GetComponent<Text>();


        Transform showDetailPanel = Root.GetChild(2);
        DetailButton = showDetailPanel.GetComponent<ShowDetailButton>();

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
        NameText.text = dataModel.MissionName + " (lv:" + Battle.GetComponent<RunBattle>().missionLevel + ")";
        LocationText.text = "  " + dataModel.Location;


        LevelOfMissionText.text = "Lv: " + Battle.GetComponent<RunBattle>().missionLevel;

        string resultText = null;

        switch (WhichWin)
        {
            case WhichWin.AllyWin:
                resultText = "[Win]";
                break;
            case WhichWin.EnemyWin:
                resultText = "[Lose]";
                break;
            case WhichWin.Draw:
                resultText = "[Draw]";
                break;
            case WhichWin.None:
                break;
            default:
                Debug.LogError(" unexpected value :" + WhichWin);
                break;
        }
        ResultText.text = resultText;

        //ShowDetailButton showDetailButton = detailButton.GetComponent<ShowDetailButton>();

        DetailButton.battle = Battle;
        DetailButton.battleLogEnhancedScrollController = BattleLogEnhancedScrollController;
        DetailButton.battleLog = BattleLog;
        DetailButton.logList = LogList;

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
    public string MissionName;
    public string Location;
    public WhichWin WhichWin;
    public string ResultText;

    //Obsolate
    public float Availability01, ContractChance01, LongTermClient01;
    public bool IsOnline;

    public float AverageScore01 { get { return (Availability01 + ContractChance01 + LongTermClient01) / 3; } }

    public void SetRandom()
    {
        Availability01 = CUtil.RandF();
        ContractChance01 = CUtil.RandF();
        LongTermClient01 = CUtil.RandF();
        IsOnline = CUtil.Rand(2) == 0;
    }
}


public class LogClientModel : LogBaseClientModel
{
    // View size related
    public bool Expanded;
    public float NonExpandedSize;
}


public class LogViewsHolder : LogClientViewsHolder<LogClientModel>
{
    public CExpandCollapseOnClick ExpandCollapseComponent;


    public override void CollectViews()
    {
        base.CollectViews();

        ExpandCollapseComponent = Root.GetComponent<CExpandCollapseOnClick>();
    }

    public override void UpdateViews(LogClientModel dataModel)
    {
        base.UpdateViews(dataModel);

        if (ExpandCollapseComponent)
        {
            ExpandCollapseComponent.expanded = dataModel.Expanded;
            ExpandCollapseComponent.nonExpandedSize = dataModel.NonExpandedSize;
        }
    }
}

