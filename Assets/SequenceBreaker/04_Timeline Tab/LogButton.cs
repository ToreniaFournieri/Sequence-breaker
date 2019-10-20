using System.Collections;
using System.Collections.Generic;
using SequenceBreaker._08_Battle;
using UnityEngine;
using UnityEngine.UI;

sealed  public class LogButton : MonoBehaviour
{
    public Button button;
    public Text orderCondition;
    public Text log;
    public Image iconImage;

    private BattleLogClass _battleLog;

    //InventoryScrollList scrollList;

    // Start is called before the first frame update
    void Start()
    {
        //button.onClick.AddListener(HandleClick);
    }
    public void Setup(BattleLogClass currentLog)
    {
        _battleLog = currentLog;
        //scrollList = currentScrollList;
        orderCondition.text = _battleLog.OrderCondition.ToString();
        //iconImage.sprite = item.icon;
        log.text = _battleLog.Log;
        //scrollList = currentScrollList;

        this.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

    }

    //public void HandleClick()
    //{
    //    scrollList.TryTransferItemToOtherInventory(item);
    //}
}
