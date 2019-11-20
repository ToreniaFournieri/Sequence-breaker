using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker.Home.EquipView
{
    [System.Serializable]
    public sealed class ItemDetailViewController : MonoBehaviour
    {
        [FormerlySerializedAs("ItemDetailViewGameObject")] public GameObject itemDetailViewGameObject;
        [FormerlySerializedAs("ItemdetailViewText")] public GameObject itemdetailViewText;
        [FormerlySerializedAs("ItemDetailViewButton")] public Button itemDetailViewButton;

        public void CloseView()
        {
            itemDetailViewGameObject.SetActive(false);
            itemdetailViewText.GetComponent<Text>().text = null;

        }

        public void OpenView(string String)
        {

            itemdetailViewText.GetComponent<Text>().text = String;
            itemDetailViewGameObject.transform.SetAsLastSibling();
            itemDetailViewGameObject.SetActive(true);


        }

    }
}
