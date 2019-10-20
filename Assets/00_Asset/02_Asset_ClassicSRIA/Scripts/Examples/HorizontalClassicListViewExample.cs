using UnityEngine;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Classic.Examples.Common;
using UnityEngine.Serialization;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples
{
    /// <summary>Same as <see cref="VerticalClassicListViewExample"/> except it's horizontal, the items are not resize-able</summary>
    public class HorizontalClassicListViewExample : ClassicSria<LogClientViewsHolder>
	{
		public RectTransform itemPrefab;
		public string[] sampleFirstNames;//, sampleLastNames;
		public string[] sampleLocations;
		[FormerlySerializedAs("demoUI")] public DemoUi demoUi;

		public List<LogBaseClientModel> Data { get; private set; }


		#region ClassicSRIA implementation
		protected override void Awake()
		{
			base.Awake();

			Data = new List<LogBaseClientModel>();
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
		}
		
		protected override LogClientViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new LogClientViewsHolder();
			instance.Init(itemPrefab, itemIndex);

			return instance;
		}

		protected override void UpdateViewsHolder(LogClientViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
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

        LogBaseClientModel CreateNewModel(int index)
		{
			var model = new LogBaseClientModel()
			{
				MissionName = sampleFirstNames[CUtil.Rand(sampleFirstNames.Length)],
				Location = sampleLocations[CUtil.Rand(sampleLocations.Length)],
			};
			model.SetRandom();

			return model;
		}
	}	
}
