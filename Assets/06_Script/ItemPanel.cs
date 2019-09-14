using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    public Button button;
    public Text nameLabel;
    public Text descriptionLabel;

	//to show item detail view, tell button objet
	public Button itemDetailViewButton;
	private ItemDetailViewController _itemDetailViewController;
	//public Image iconImage;

	private Item item;
    private InventoryScrollList scrollList;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(HandleClick);
    }
    public void Setup(Item currentItem, InventoryScrollList currentScrollList, ItemDetailViewController itemDetailViewController)
    {
        item = currentItem;
        scrollList = currentScrollList;
        nameLabel.text = item.itemName;
        //iconImage.sprite = item.icon;
        descriptionLabel.text = item.itemDescription;
        //scrollList = currentScrollList;

        _itemDetailViewController = itemDetailViewController;

        this.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

    }

    public void HandleClick()
    {
        scrollList.TryTransferItemToOtherInventory(item);
    }

    public void ItemDetailIsClicked()
    {
        _itemDetailViewController.OpenView(item.GetItemDetailDescription());
    }
}
