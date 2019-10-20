using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
{
    public class ListItem3 : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Text mDescText;
        int _mItemIndex = -1;
        public Toggle mToggle;
        public void Init()
        {
            mToggle.onValueChanged.AddListener(OnToggleValueChanged);

        }

        void OnToggleValueChanged(bool check)
        {
            ItemData data = DataSourceMgr.Get.GetItemDataByIndex(_mItemIndex);
            if (data == null)
            {
                return;
            }
            data.MChecked = check;
        }

        public void SetItemData(ItemData itemData,int itemIndex)
        {
            _mItemIndex = itemIndex;
            mNameText.text = itemData.MName;
            mDescText.text = itemData.MDesc;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.MIcon);
            mToggle.isOn = itemData.MChecked;
        }


    }
}
