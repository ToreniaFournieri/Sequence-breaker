using System.Collections;
using System.Collections.Generic;
using _00_Asset._02_ClassicSRIA.Scripts;
using _00_Asset._02_ClassicSRIA.Scripts.Util;
using SequenceBreaker._04_Timeline_Tab.Log.BattleLog;
using SequenceBreaker._08_Battle._2_BeforeBattle;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._04_Timeline_Tab.Log
{
    public sealed class LogListSria : ClassicSria<LogViewsHolder>, CExpandCollapseOnClick.ISizeChangesHandler

    {
        //for battle calculation
        public List<GameObject> battleList;
        //for log 
        public BattleLogEnhancedScrollController battleLogEnhancedScrollController;
        public GameObject battleLog;
        public GameObject logList;


        public GameObject transparentMessageController;
        public RectTransform itemPrefab;

//    [FormerlySerializedAs("demoUI")] public DemoUi demoUi;

        public List<LogClientModel> Data { get; private set; }

        LayoutElement _prefabLayoutElement;
        // Used to quickly retrieve the views holder given the gameObject
        readonly Dictionary<RectTransform, LogViewsHolder> _mapRootToViewsHolder = new Dictionary<RectTransform, LogViewsHolder>();


        #region ClassicSRIA implementation
        protected override void Awake()
        {
            base.Awake();

            Data = new List<LogClientModel>();
            _prefabLayoutElement = itemPrefab.GetComponent<LayoutElement>();
        }

        protected override void Start()
        {
            base.Start();

            if (battleList.Count > 0)
            {
                ChangeModelsAndReset(battleList.Count);
            }

            //demoUI.setCountButton.onClick.AddListener(OnItemCountChangeRequested);
            //demoUI.scrollToButton.onClick.AddListener(OnScrollToRequested);
            //demoUI.addOneTailButton.onClick.AddListener(() => OnAddItemRequested(true));
            //demoUI.addOneHeadButton.onClick.AddListener(() => OnAddItemRequested(false));
            //demoUI.removeOneTailButton.onClick.AddListener(() => OnRemoveItemRequested(true));
            //demoUI.removeOneHeadButton.onClick.AddListener(() => OnRemoveItemRequested(false));



            StartCoroutine(DelayedClick());
        }

        //public void AddBattleList(GameObject _battle)
        //{
        //    battleList.Add(_battle);
        //}


        IEnumerator DelayedClick()
        {
            yield return new WaitForSeconds(.4f);

            if (viewsHolders.Count > 0)
                viewsHolders[Mathf.Min(3, Data.Count - 1)].ExpandCollapseComponent.OnClicked();
        }

        protected override LogViewsHolder CreateViewsHolder(int itemIndex)
        {
            var instance = new LogViewsHolder();
            instance.Init(itemPrefab, itemIndex);
            instance.ExpandCollapseComponent.sizeChangesHandler = this;

            //Set battle gameObject to each instanced itemPrefab
            instance.Battle = battleList[itemIndex];
            //instance.battleLogEnhancedScrollController = battleLogEnhancedScrollController;
            instance.TransparentMessageController = transparentMessageController;

            instance.WhichWin = battleList[itemIndex].GetComponent<RunBattle>().whichWin;
            instance.BattleLogEnhancedScrollController = battleLogEnhancedScrollController;
            instance.BattleLog = battleLog;
            instance.LogList = logList;

            _mapRootToViewsHolder[instance.Root] = instance;

            return instance;
        }


        protected override void UpdateViewsHolder(LogViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
        #endregion

        #region events from DrawerCommandPanel
//        void OnAddItemRequested(bool atEnd)
//        {
//            int index = atEnd ? Data.Count : 0;
//            Data.Insert(index, CreateNewModel(index));
////        InsertItems(index, 1, demoUi.freezeContentEndEdge.isOn);
//        }
//        void OnRemoveItemRequested(bool fromEnd)
//        {
//            if (Data.Count == 0)
//                return;
//
//            int index = fromEnd ? Data.Count - 1 : 0;
//
//            Data.RemoveAt(index);
////        RemoveItems(index, 1, demoUi.freezeContentEndEdge.isOn);
//        }

//        void OnItemCountChangeRequested()
//        {
////        ChangeModelsAndReset(demoUi.SetCountValue);
//        }
//        void OnScrollToRequested()
//        {
////        if (demoUi.ScrollToValue >= Data.Count)
////            return;
////
////        demoUi.scrollToButton.interactable = false;
////        bool started = SmoothScrollTo(demoUi.ScrollToValue, .75f, .5f, .5f, () => demoUi.scrollToButton.interactable = true);
////        if (!started)
////            demoUi.scrollToButton.interactable = true;
//        }
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

        // modify ChangeModelsAndReset to public  
        public void ChangeModelsAndReset(int newCount)
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

        LogClientModel CreateNewModel(int index)
        {


            var model = new LogClientModel()
            {
                MissionName = battleList[index].GetComponent<RunBattle>().missionText,
                Location = battleList[index].GetComponent<RunBattle>().location,
                WhichWin = battleList[index].GetComponent<RunBattle>().whichWin,
                NonExpandedSize = _prefabLayoutElement.preferredHeight
            };
            model.SetRandom();



            return model;
        }

    }
}
