using System.Collections;
using System.Collections.Generic;
using _00_Asset._02_ClassicSRIA.Scripts;
using _00_Asset._02_ClassicSRIA.Scripts.Util;
using SequenceBreaker.Play.Prepare;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Play.MissionView
{
    public sealed class MissionListSria : ClassicSria<MissionViewsHolder>, CExpandCollapseOnClick.ISizeChangesHandler

    {
        //for battle calculation
        public List<RunBattle> runBattleList;

        public int levelRange;

        // Mission Controller (ally unit list and battle engine)
        public MissionController missionController;

        public RectTransform itemPrefab;

        private List<ExpandableSimpleClientModel> Data { get; set; }

        LayoutElement _prefabLayoutElement;
        // Used to quickly retrieve the views holder given the gameObject
        private readonly Dictionary<RectTransform, MissionViewsHolder> _mapRootToViewsHolder = new Dictionary<RectTransform, MissionViewsHolder>();


        #region ClassicSRIA implementation
        protected override void Awake()
        {
            base.Awake();

            Data = new List<ExpandableSimpleClientModel>();
            _prefabLayoutElement = itemPrefab.GetComponent<LayoutElement>();
        }

        protected override void Start()
        {
            base.Start();
            runBattleList = missionController.GetRunBattleList();

            if (runBattleList.Count > 0)
            {
                ChangeModelsAndReset(runBattleList.Count);
            }

            //demoUI.setCountButton.onClick.AddListener(OnItemCountChangeRequested);
            //demoUI.scrollToButton.onClick.AddListener(OnScrollToRequested);
            //demoUI.addOneTailButton.onClick.AddListener(() => OnAddItemRequested(true));
            //demoUI.addOneHeadButton.onClick.AddListener(() => OnAddItemRequested(false));
            //demoUI.removeOneTailButton.onClick.AddListener(() => OnRemoveItemRequested(true));
            //demoUI.removeOneHeadButton.onClick.AddListener(() => OnRemoveItemRequested(false));



            StartCoroutine(DelayedClick());



        }

        IEnumerator DelayedClick()
        {
            yield return new WaitForSeconds(.4f);

            if (viewsHolders.Count > 0)
                viewsHolders[Mathf.Min(3, Data.Count - 1)].ExpandCollapseComponent.OnClicked();
        }

        protected override MissionViewsHolder CreateViewsHolder(int itemIndex)
        {
            var instance = new MissionViewsHolder();
            instance.Init(itemPrefab, itemIndex);
            instance.ExpandCollapseComponent.sizeChangesHandler = this;

            //Set battle gameObject to each instanced itemPrefab
            instance.RunBattle = runBattleList[itemIndex];
            instance.MissionController = missionController;

            instance.LevelOfMissionSlider.minValue = runBattleList[itemIndex].mission.levelInitial;
            instance.LevelOfMissionSlider.maxValue = runBattleList[itemIndex].mission.levelInitial + levelRange;
            instance.LevelOfMissionSlider.value = runBattleList[itemIndex].mission.levelInitial;


            _mapRootToViewsHolder[instance.Root] = instance;

            return instance;
        }

        protected override void UpdateViewsHolder(MissionViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
        #endregion

        #region events from DrawerCommandPanel

        #endregion

        #region CExpandCollapseOnClick.ISizeChangesHandler implementation
        public bool HandleSizeChangeRequest(RectTransform rt, float newSize)
        {
            _mapRootToViewsHolder[rt].LayoutElement.preferredHeight = newSize;
            return true;
        }

        public void OnExpandedStateChanged(RectTransform rt, bool expanded)
        {
            var itemIndex = _mapRootToViewsHolder[rt].ItemIndex;
            Data[itemIndex].Expanded = expanded;
        }
        #endregion

        private void ChangeModelsAndReset(int newCount)
        {
            Data.Clear();
            Data.Capacity = newCount;
            for (int i = 0; i < newCount; i++)
            {
                var model = CreateNewModel(i);
                Data.Add(model);
            }

            ResetItems(Data.Count);
        }

        ExpandableSimpleClientModel CreateNewModel(int index)
        {
            var model = new ExpandableSimpleClientModel()
            {
                MissionName = runBattleList[index].mission.missionName,
                Location = runBattleList[index].mission.locationString,
                NonExpandedSize = _prefabLayoutElement.preferredHeight
            };
            model.SetRandom();



            return model;
        }

    }




    public class SimpleExpandableClientViewsHolder : BaseClientViewsHolder<ExpandableSimpleClientModel>
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