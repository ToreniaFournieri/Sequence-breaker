using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._04_Home.EquipView.Character
{
    public class CharacterHeaderSizeChanger : MonoBehaviour
    {
        public bool isExpand = true;
        public Button expandButton;

        //switch both of them
        public GameObject headerGameObject;
        public GameObject characterItemListGameObject;

        private void Start()
        {

            HeaderStatusShowHide();
        }
        

        private void Init()
        {
            expandButton.transform.Rotate(isExpand ? new Vector3(0, 0, -180) : new Vector3(0, 0, 180));
        }

        public void HeaderStatusShowHide()
        {
            isExpand = !isExpand;

            if (isExpand)
            {
                characterItemListGameObject.transform.SetAsLastSibling();
            }
            else
            {
                headerGameObject.transform.SetAsLastSibling();
            }

            Init();

        }
        
    }
}
