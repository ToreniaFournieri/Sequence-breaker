using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic;
using frame8.ScrollRectItemsAdapter.Classic.Util;
using UnityEngine.Serialization;


public sealed class MissionListSria : ClassicSria<MissionViewsHolder>, CExpandCollapseOnClick.ISizeChangesHandler

{
    //for battle calculation
    public List<GameObject> battleList;
    public List<RunBattle> runBattleList;

    // Mission Controller (ally unit list and battle engine)
    public MissionController missionController;

    public RectTransform itemPrefab;
    [FormerlySerializedAs("demoUI")] public DemoUi demoUi;

    public List<ExpandableSimpleClientModel> Data { get; private set; }

    LayoutElement _prefabLayoutElement;
    // Used to quickly retrieve the views holder given the gameObject
    Dictionary<RectTransform, MissionViewsHolder> _mapRootToViewsHolder = new Dictionary<RectTransform, MissionViewsHolder>();


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

        if (battleList.Count > 0)
        {
            ChangeModelsAndReset(battleList.Count);
        }
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
        instance.Runbattle = runBattleList[itemIndex];
        instance.MissionController = missionController;

        instance.LevelOfMissionSlider.minValue = runBattleList[itemIndex].missionLevel;
        instance.LevelOfMissionSlider.maxValue = runBattleList[itemIndex].missionLevel + 30;
        instance.LevelOfMissionSlider.value = runBattleList[itemIndex].missionLevel;


        _mapRootToViewsHolder[instance.Root] = instance;

        return instance;
    }

    protected override void UpdateViewsHolder(MissionViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
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
            MissionName = battleList[index].GetComponent<RunBattle>().missionText,
            Location = battleList[index].GetComponent<RunBattle>().location,

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