using System.Collections.Generic;
using _00_Asset._02_Asset_ClassicSRIA.Scripts.Examples.Common;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using CUtil = SequenceBreaker._07_Play._1_View.CUtil;

namespace _00_Asset._02_Asset_ClassicSRIA.Scripts.Examples
{
    public class ClassicGridViewExample : ClassicSria<CellViewsHolder>
	{
		public RectTransform itemPrefab;
		public Sprite[] availableImages;
		[FormerlySerializedAs("demoUI")] public DemoUi demoUi;

		public List<CellModel> Data { get; private set; }


		#region ClassicSRIA implementation
		protected override void Awake()
		{
			base.Awake();

			Data = new List<CellModel>();
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
		
		protected override CellViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new CellViewsHolder();
			instance.Init(itemPrefab, itemIndex);

			return instance;
		}

		protected override void UpdateViewsHolder(CellViewsHolder vh)
		{
			var model = Data[vh.ItemIndex];
			vh.TitleText.text =  "[#"+vh.ItemIndex+"] " + model.Title;
			vh.Image.sprite = availableImages[model.ImageIndex];
		}
		#endregion

		#region events from DrawerCommandPanel
		void OnAddItemRequested(bool atEnd)
		{
			int index = atEnd ? Data.Count : 0;
			Data.Insert(index, CreateNewModel());
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
				var model = CreateNewModel();
				Data.Add(model);
			}

			ResetItems(Data.Count);
		}

		CellModel CreateNewModel()
		{
			int imgIdx = CUtil.Rand(availableImages.Length);
			var model = new CellModel()
			{
				Title = "Image "+ imgIdx,
				ImageIndex = imgIdx,
			};

			return model;
		}
	}


	public class CellModel
	{
		public string Title;
		public int ImageIndex;
	}


	public class CellViewsHolder : CAbstractViewsHolder
	{
		public Text TitleText;
		public Image Image;


		public override void CollectViews()
		{
			base.CollectViews();

			TitleText = Root.Find("TitleText").GetComponent<Text>();
			Image = Root.Find("ImagePanel/Image").GetComponent<Image>();
		}
	}
}
