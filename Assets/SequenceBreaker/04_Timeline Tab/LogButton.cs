using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed  public class LogButton : MonoBehaviour
{
    public Button button;
    public Text orderCondition;
    public Text log;
    public Image iconImage;

    private BattleLogClass battleLog;

    //InventoryScrollList scrollList;

    // Start is called before the first frame update
    void Start()
    {
        //button.onClick.AddListener(HandleClick);
    }
    public void Setup(BattleLogClass currentLog)
    {
        battleLog = currentLog;
        //scrollList = currentScrollList;
        orderCondition.text = battleLog.OrderCondition.ToString();
        //iconImage.sprite = item.icon;
        log.text = battleLog.Log;
        //scrollList = currentScrollList;

        this.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

    }

    //public void HandleClick()
    //{
    //    scrollList.TryTransferItemToOtherInventory(item);
    //}
}
