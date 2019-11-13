using _00_Asset._02_ClassicSRIA.Scripts;
using _00_Asset._02_ClassicSRIA.Scripts.Util;
using SequenceBreaker._03_Controller.Play;
using SequenceBreaker._08_Battle._2_BeforeBattle;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._07_Play._1_View
{
    public class BaseClientViewsHolder<TClientModel> : CAbstractViewsHolder where TClientModel : SimpleClientModel
    {
        public LayoutElement LayoutElement;
        public Image AverageScoreFillImage;
        public Text NameText, LocationText;
        //averageScoreText;

        public Slider LevelOfMissionSlider;

        public MissionController MissionController;

        //Battle object
        public GameObject Battle;
        public RunBattle Runbattle;

        public GoScript GoScript;

        public Text LevelOfMissionText;


        /// <inheritdoc/>
        public override void CollectViews()
        {
            base.CollectViews();

            LayoutElement = Root.GetComponent<LayoutElement>();

            var mainPanel = Root.GetChild(0);
            LevelOfMissionText = mainPanel.Find("MissionImage/LevelText").GetComponent<Text>();
            NameText = mainPanel.Find("NameAndLocationPanel/MissionText").GetComponent<Text>();
            LocationText = mainPanel.Find("NameAndLocationPanel/LocationText").GetComponent<Text>();

            var secondPanel = Root.GetChild(1);
            LevelOfMissionSlider = secondPanel.Find("AvailabilityPanel/Slider").GetComponent<Slider>();

        }

        public void SetBattle()
        {
            //Set battle gameobject to activate GO button
            var go = Root.GetChild(2);
            GoScript = go.GetComponent<GoScript>();

            GoScript.runBattle = Runbattle;
            GoScript.missionController = MissionController;


        }

        public virtual void UpdateViews(TClientModel dataModel)
        {
            NameText.text = dataModel.MissionName + "(#" + ItemIndex + ")";
            LocationText.text = "  " + dataModel.Location;
            SetBattle();



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
        public string MissionName;
        public string Location;

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


    public sealed class ExpandableSimpleClientModel : SimpleClientModel
    {
        // View size related
        public bool Expanded;
        public float NonExpandedSize;
    }


    public sealed class MissionViewsHolder : BaseClientViewsHolder<ExpandableSimpleClientModel>
    {
        public CExpandCollapseOnClick ExpandCollapseComponent;



        public override void CollectViews()
        {
            base.CollectViews();

            ExpandCollapseComponent = Root.GetComponent<CExpandCollapseOnClick>();
        }

        public override void UpdateViews(ExpandableSimpleClientModel dataModel)
        {
            base.UpdateViews(dataModel);

            if (ExpandCollapseComponent)
            {
                ExpandCollapseComponent.expanded = dataModel.Expanded;
                ExpandCollapseComponent.nonExpandedSize = dataModel.NonExpandedSize;
            }
        }
    }
}