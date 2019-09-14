using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic;
using frame8.ScrollRectItemsAdapter.Classic.Util;


public class BaseClientViewsHolder<TClientModel> : CAbstractViewsHolder where TClientModel : SimpleClientModel
{
    public LayoutElement layoutElement;
    public Image averageScoreFillImage;
    public Text nameText, locationText, averageScoreText;
    public RectTransform availability01Slider, contractChance01Slider, longTermClient01Slider;
    public Text statusText;


    /// <inheritdoc/>
    public override void CollectViews()
    {
        base.CollectViews();

        layoutElement = root.GetComponent<LayoutElement>();

        var mainPanel = root.GetChild(0);
        statusText = mainPanel.Find("MissionImage/LevelText").GetComponent<Text>();
        nameText = mainPanel.Find("NameAndLocationPanel/NameText").GetComponent<Text>();
        locationText = mainPanel.Find("NameAndLocationPanel/LocationText").GetComponent<Text>();

        //var ratingPanel = root.Find("RatingPanel/Panel").GetComponent<RectTransform>();
        //averageScoreFillImage = ratingPanel.Find("Foreground").GetComponent<Image>();
        //averageScoreText = ratingPanel.Find("Text").GetComponent<Text>();

        var ratingBreakdownPanel = root.Find("RatingBreakdownPanel").GetComponent<RectTransform>();
        availability01Slider = ratingBreakdownPanel.Find("AvailabilityPanel/Slider").GetComponent<RectTransform>();
        contractChance01Slider = ratingBreakdownPanel.Find("ContractChancePanel/Slider").GetComponent<RectTransform>();
        longTermClient01Slider = ratingBreakdownPanel.Find("LongTermClientPanel/Slider").GetComponent<RectTransform>();
    }

    public virtual void UpdateViews(TClientModel dataModel)
    {
        nameText.text = dataModel.clientName + "(#" + ItemIndex + ")";
        locationText.text = "  " + dataModel.location;
        UpdateScores(dataModel);
        if (dataModel.isOnline)
        {
            statusText.text = "Online";
            statusText.color = Color.green;
        }
        else
        {
            statusText.text = "Offline";
            statusText.color = Color.white * .8f;
        }
    }

    void UpdateScores(SimpleClientModel dataModel)
    {
        var scale = availability01Slider.localScale;
        scale.x = dataModel.availability01;
        availability01Slider.localScale = scale;

        scale = contractChance01Slider.localScale;
        scale.x = dataModel.contractChance01;
        contractChance01Slider.localScale = scale;

        scale = longTermClient01Slider.localScale;
        scale.x = dataModel.longTermClient01;
        longTermClient01Slider.localScale = scale;

        //float avgScore = dataModel.AverageScore01;
        //averageScoreFillImage.fillAmount = avgScore;
        //averageScoreText.text = (int)(avgScore * 100) + "%";
    }
}

public class SimpleClientViewsHolder : BaseClientViewsHolder<SimpleClientModel>
{

}

public static class CUtil
{

    // Utility randomness methods
    public static int Rand(int maxExcl) { return UnityEngine.Random.Range(0, maxExcl); }
    public static float RandF(float max = 1f) { return UnityEngine.Random.Range(0, max); }
}

public class SimpleClientModel
{
    public string clientName;
    public string location;
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


public class ExpandableSimpleClientModel : SimpleClientModel
{
    // View size related
    public bool expanded;
    public float nonExpandedSize;
}


public class MyItemViewsHolder : BaseClientViewsHolder<ExpandableSimpleClientModel>
{
    public CExpandCollapseOnClick expandCollapseComponent;


    public override void CollectViews()
    {
        base.CollectViews();

        expandCollapseComponent = root.GetComponent<CExpandCollapseOnClick>();
    }

    public override void UpdateViews(ExpandableSimpleClientModel dataModel)
    {
        base.UpdateViews(dataModel);

        if (expandCollapseComponent)
        {
            expandCollapseComponent.expanded = dataModel.expanded;
            expandCollapseComponent.nonExpandedSize = dataModel.nonExpandedSize;
        }
    }
}
