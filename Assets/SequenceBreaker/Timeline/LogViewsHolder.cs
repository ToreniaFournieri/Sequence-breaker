using _00_Asset._02_ClassicSRIA.Scripts;
using _00_Asset._02_ClassicSRIA.Scripts.Util;
using SequenceBreaker.Environment;
using SequenceBreaker.Play.MissionView;
using SequenceBreaker.Play.Prepare;
using SequenceBreaker.Timeline.BattleLogView;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Timeline
{
    public class LogClientViewsHolder<TClientModel> : CAbstractViewsHolder where TClientModel : LogBaseClientModel
    {
        public LayoutElement LayoutElement;
        public Image AverageScoreFillImage;
        public Text NameText, LocationText;

        public Slider LevelOfMissionSlider;
        public GameObject TransparentMessageController; // to display transparent message

        //Show detail button objects
        public ShowDetailButton DetailButton;

        // Log related values
        public BattleLogEnhancedScrollController BattleLogEnhancedScrollController; // to pass the log value
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
            NameText = mainPanel.Find("NameAndLocationPanel/MissionText").GetComponent<Text>();
            LocationText = mainPanel.Find("NameAndLocationPanel/LocationText").GetComponent<Text>();


            Transform showDetailPanel = Root.GetChild(2);
            DetailButton = showDetailPanel.GetComponent<ShowDetailButton>();

        }
        

        public virtual void UpdateViews(TClientModel dataModel)
        {
            NameText.text = Battle.GetComponent<RunBattle>().currentMissionName + " (lv:" + Battle.GetComponent<RunBattle>().currentLevel + ")";
            LocationText.text = "  " + dataModel.Location;


            LevelOfMissionText.text = "Lv: " + Battle.GetComponent<RunBattle>().currentLevel;

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
            DetailButton.battle = Battle;
            DetailButton.battleLogEnhancedScrollController = BattleLogEnhancedScrollController;
            DetailButton.battleLog = BattleLog;
            DetailButton.logList = LogList;

        }

    }

    public class LogClientViewsHolder : LogClientViewsHolder<LogBaseClientModel>
    {

    }


    public class LogBaseClientModel
    {
        public string MissionName;
        public string Location;
        public WhichWin WhichWin;
        public string ResultText;

        //Obsolete
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
}