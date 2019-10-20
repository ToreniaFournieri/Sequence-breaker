using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
sealed public class ItemDetailViewController : MonoBehaviour
{
    public GameObject ItemDetailViewGameObject;
    public GameObject ItemdetailViewText;
    public Button ItemDetailViewButton;

    public void CloseView()
    {
        ItemDetailViewGameObject.SetActive(false);
        ItemdetailViewText.GetComponent<Text>().text = null;

    }

    public void OpenView(string String)
    {

        ItemdetailViewText.GetComponent<Text>().text = String;
        ItemDetailViewGameObject.transform.SetAsLastSibling();
        ItemDetailViewGameObject.SetActive(true);


    }

}
