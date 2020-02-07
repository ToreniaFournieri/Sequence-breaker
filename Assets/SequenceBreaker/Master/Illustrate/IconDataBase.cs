using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Master.Illustrate
{


    public class IconDataBase : MonoBehaviour
    {

        [SerializeField]
        public List<Sprite> IconSpriteList;
        public Sprite DefaultSprite;


        public static IconDataBase instance;

        private void Awake()
        {
            //Debug.Log("ItemDataBase.Awake() GetInstanceID=" + this.GetInstanceID().ToString());

            if (instance == null)
            {
                instance = this;  //This is the first Singleton instance. Retain a handle to it.
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this); //This is a duplicate Singleton. Destroy this instance.
                }
                else
                {
                    //Existing Singleton instance found. All is good. No change.
                }
            }

            DontDestroyOnLoad(gameObject);
        }


        public Sprite GetSprite(string spriteString)
        {
            Sprite selectedSprite = null;

            foreach (Sprite icon in IconSpriteList)
            {
                if(spriteString == icon.name)
                {
                    selectedSprite = icon;
                }
            }

            if (selectedSprite == null)
            {
                selectedSprite = DefaultSprite;
            }

            return selectedSprite;
        }

    }
}