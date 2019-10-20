using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic.Util;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
	public class BaseClientViewsHolder<TClientModel> : CAbstractViewsHolder where TClientModel : SimpleClientModel
	{
		public LayoutElement LayoutElement;
		public Image AverageScoreFillImage;
		public Text NameText, LocationText, AverageScoreText;
		public RectTransform Availability01Slider, ContractChance01Slider, LongTermClient01Slider;
		public Text StatusText;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			LayoutElement = Root.GetComponent<LayoutElement>();

			var mainPanel = Root.GetChild(0);
			StatusText = mainPanel.Find("AvatarPanel/StatusText").GetComponent<Text>();
			NameText = mainPanel.Find("NameAndLocationPanel/NameText").GetComponent<Text>();
			LocationText = mainPanel.Find("NameAndLocationPanel/LocationText").GetComponent<Text>();

			var ratingPanel = Root.Find("RatingPanel/Panel").GetComponent<RectTransform>();
			AverageScoreFillImage = ratingPanel.Find("Foreground").GetComponent<Image>();
			AverageScoreText = ratingPanel.Find("Text").GetComponent<Text>();

			var ratingBreakdownPanel = Root.Find("RatingBreakdownPanel").GetComponent<RectTransform>();
			Availability01Slider = ratingBreakdownPanel.Find("AvailabilityPanel/Slider").GetComponent<RectTransform>();
			ContractChance01Slider = ratingBreakdownPanel.Find("ContractChancePanel/Slider").GetComponent<RectTransform>();
			LongTermClient01Slider = ratingBreakdownPanel.Find("LongTermClientPanel/Slider").GetComponent<RectTransform>();
		}

		public virtual void UpdateViews(TClientModel dataModel)
		{
			NameText.text = dataModel.ClientName + "(#" + ItemIndex + ")";
			LocationText.text = "  " + dataModel.Location;
			UpdateScores(dataModel);
			if (dataModel.IsOnline)
			{
				StatusText.text = "Online";
				StatusText.color = Color.green;
			}
			else
			{
				StatusText.text = "Offline";
				StatusText.color = Color.white * .8f;
			}
		}

		void UpdateScores(SimpleClientModel dataModel)
		{
			var scale = Availability01Slider.localScale;
			scale.x = dataModel.Availability01;
			Availability01Slider.localScale = scale;

			scale = ContractChance01Slider.localScale;
			scale.x = dataModel.ContractChance01;
			ContractChance01Slider.localScale = scale;

			scale = LongTermClient01Slider.localScale;
			scale.x = dataModel.LongTermClient01;
			LongTermClient01Slider.localScale = scale;

			float avgScore = dataModel.AverageScore01;
			AverageScoreFillImage.fillAmount = avgScore;
			AverageScoreText.text = (int)(avgScore * 100) + "%";
		}
	}

	public class SimpleClientViewsHolder : BaseClientViewsHolder<SimpleClientModel>
	{

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
