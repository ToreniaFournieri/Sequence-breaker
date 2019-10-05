using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic;
using frame8.ScrollRectItemsAdapter.Classic.Util;



public class MissionListSRIA : ClassicSRIA<MissionViewsHolder>, CExpandCollapseOnClick.ISizeChangesHandler

{
    //for battle calculation
    public List<GameObject> battleList;
    //for adding BattleLog to log list 
    public LogListSRIA logList;
    //for Inventory update
    public GameObject inventoryManager;


    //for party status update
    public GameObject partyStatusIcons;

    //for transparent message
    public GameObject transparentMessageController;

    public RectTransform itemPrefab;
    public DemoUI demoUI;

    public List<ExpandableSimpleClientModel> Data { get; private set; }

    LayoutElement _PrefabLayoutElement;
    // Used to quickly retrieve the views holder given the gameobject
    Dictionary<RectTransform, MissionViewsHolder> _MapRootToViewsHolder = new Dictionary<RectTransform, MissionViewsHolder>();


    #region ClassicSRIA implementation
    protected override void Awake()
    {
        base.Awake();

        Data = new List<ExpandableSimpleClientModel>();
        _PrefabLayoutElement = itemPrefab.GetComponent<LayoutElement>();
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

    IEnumerator DelayedClick()
    {
        yield return new WaitForSeconds(.4f);

        if (viewsHolders.Count > 0)
            viewsHolders[Mathf.Min(3, Data.Count - 1)].expandCollapseComponent.OnClicked();
    }

    protected override MissionViewsHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new MissionViewsHolder();
        instance.Init(itemPrefab, itemIndex);
        instance.expandCollapseComponent.sizeChangesHandler = this;

        //Set battle gameObject to each instanced itemPrefab
        instance.battle = battleList[itemIndex];
        //instance.battleLogEnhancedScrollController = battleLogEnhancedScrollController;
        instance.transparentMessageController = transparentMessageController;
        instance.inventoryManager = inventoryManager;
        instance.partyStatusIcons = partyStatusIcons;

        instance.logList = logList;

        _MapRootToViewsHolder[instance.root] = instance;

        return instance;
    }

    protected override void UpdateViewsHolder(MissionViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
    #endregion

    #region events from DrawerCommandPanel
    void OnAddItemRequested(bool atEnd)
    {
        int index = atEnd ? Data.Count : 0;
        Data.Insert(index, CreateNewModel(index));
        InsertItems(index, 1, demoUI.freezeContentEndEdge.isOn);
    }
    void OnRemoveItemRequested(bool fromEnd)
    {
        if (Data.Count == 0)
            return;

        int index = fromEnd ? Data.Count - 1 : 0;

        Data.RemoveAt(index);
        RemoveItems(index, 1, demoUI.freezeContentEndEdge.isOn);
    }
    void OnItemCountChangeRequested() { ChangeModelsAndReset(demoUI.SetCountValue); }
    void OnScrollToRequested()
    {
        if (demoUI.ScrollToValue >= Data.Count)
            return;

        demoUI.scrollToButton.interactable = false;
        bool started = SmoothScrollTo(demoUI.ScrollToValue, .75f, .5f, .5f, () => demoUI.scrollToButton.interactable = true);
        if (!started)
            demoUI.scrollToButton.interactable = true;
    }
    #endregion

    #region CExpandCollapseOnClick.ISizeChangesHandler implementation
    public bool HandleSizeChangeRequest(RectTransform rt, float newSize)
    {
        _MapRootToViewsHolder[rt].layoutElement.preferredHeight = newSize;
        return true;
    }

    public void OnExpandedStateChanged(RectTransform rt, bool expanded)
    {
        var itemIndex = _MapRootToViewsHolder[rt].ItemIndex;
        Data[itemIndex].expanded = expanded;
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
            missionName = battleList[index].GetComponent<RunBattle>().missionText,
            location = battleList[index].GetComponent<RunBattle>().location,

            nonExpandedSize = _PrefabLayoutElement.preferredHeight
        };
        model.SetRandom();



        return model;
    }

}




public class SimpleExpandableClientViewsHolder : BaseClientViewsHolder<ExpandableSimpleClientModel>
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