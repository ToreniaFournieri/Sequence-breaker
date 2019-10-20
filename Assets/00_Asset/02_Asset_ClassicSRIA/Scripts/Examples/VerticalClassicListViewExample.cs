using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Classic.Examples.Common;
using frame8.ScrollRectItemsAdapter.Classic.Util;
using System;
using System.Collections;
using UnityEngine.Serialization;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples
{
    /// <summary>Class (initially) implemented during this YouTube tutorial: https://youtu.be/aoqq_j-aV8I (which is now too old to relate). It demonstrates a simple use case with items that expand/collapse on click</summary>
    public class VerticalClassicListViewExample : ClassicSria<SimpleExpandableClientViewsHolder>, CExpandCollapseOnClick.ISizeChangesHandler
    {
        public RectTransform itemPrefab;
        public string[] sampleFirstNames;//, sampleLastNames;
        public string[] sampleLocations;
        [FormerlySerializedAs("demoUI")] public DemoUi demoUi;

        public List<ExpandableSimpleClientModel> Data { get; private set; }

        LayoutElement _prefabLayoutElement;
        // Used to quickly retrieve the views holder given the gameobject
        Dictionary<RectTransform, SimpleExpandableClientViewsHolder> _mapRootToViewsHolder = new Dictionary<RectTransform, SimpleExpandableClientViewsHolder>();


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

            ChangeModelsAndReset(demoUi.SetCountValue);

            demoUi.setCountButton.onClick.AddListener(OnItemCountChangeRequested);
            demoUi.scrollToButton.onClick.AddListener(OnScrollToRequested);
            demoUi.addOneTailButton.onClick.AddListener(() => OnAddItemRequested(true));
            demoUi.addOneHeadButton.onClick.AddListener(() => OnAddItemRequested(false));
            demoUi.removeOneTailButton.onClick.AddListener(() => OnRemoveItemRequested(true));
            demoUi.removeOneHeadButton.onClick.AddListener(() => OnRemoveItemRequested(false));

            StartCoroutine(DelayedClick());
        }

        IEnumerator DelayedClick()
        {
            yield return new WaitForSeconds(.4f);

            if (viewsHolders.Count > 0)
                viewsHolders[Mathf.Min(3, Data.Count - 1)].ExpandCollapseComponent.OnClicked();
        }

        protected override SimpleExpandableClientViewsHolder CreateViewsHolder(int itemIndex)
        {
            var instance = new SimpleExpandableClientViewsHolder();
            instance.Init(itemPrefab, itemIndex);
            instance.ExpandCollapseComponent.sizeChangesHandler = this;
            _mapRootToViewsHolder[instance.Root] = instance;

            return instance;
        }

        protected override void UpdateViewsHolder(SimpleExpandableClientViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
        #endregion

        #region events from DrawerCommandPanel
        void OnAddItemRequested(bool atEnd)
        {
            int index = atEnd ? Data.Count : 0;
            Data.Insert(index, CreateNewModel(index));
            InsertItems(index, 1, demoUi.freezeContentEndEdge.isOn);
        }
        void OnRemoveItemRequested(bool fromEnd)
        {
            if (Data.Count == 0)
                return;

            int index = fromEnd ? Data.Count - 1 : 0;

            Data.RemoveAt(index);
            RemoveItems(index, 1, demoUi.freezeContentEndEdge.isOn);
        }
        void OnItemCountChangeRequested() { ChangeModelsAndReset(demoUi.SetCountValue); }
        void OnScrollToRequested()
        {
            if (demoUi.ScrollToValue >= Data.Count)
                return;

            demoUi.scrollToButton.interactable = false;
            bool started = SmoothScrollTo(demoUi.ScrollToValue, .75f, .5f, .5f, () => demoUi.scrollToButton.interactable = true);
            if (!started)
                demoUi.scrollToButton.interactable = true;
        }
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

        void ChangeModelsAndReset(int newCount)
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
                MissionName = sampleFirstNames[CUtil.Rand(sampleFirstNames.Length)],
                Location = sampleLocations[CUtil.Rand(sampleLocations.Length)],
                NonExpandedSize = _prefabLayoutElement.preferredHeight
            };
            model.SetRandom();

            return model;
        }
    }
}
