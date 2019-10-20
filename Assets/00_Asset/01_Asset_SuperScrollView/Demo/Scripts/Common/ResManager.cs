using System.Collections.Generic;
using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
{
    public class ResManager : MonoBehaviour
    {
        public Sprite[] spriteObjArray;
        // Use this for initialization
        static ResManager _instance = null;

        string[] _mWordList;

        Dictionary<string, Sprite> _spriteObjDict = new Dictionary<string, Sprite>();

        public static ResManager Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType<ResManager>();
                }
                return _instance;
            }

        }


        void InitData()
        {
            _spriteObjDict.Clear();
            foreach (Sprite sp in spriteObjArray)
            {
                _spriteObjDict[sp.name] = sp;
            }
        }

        void Awake()
        {
            _instance = null;
            InitData();
        }

        public Sprite GetSpriteByName(string spriteName)
        {
            Sprite ret = null;
            if (_spriteObjDict.TryGetValue(spriteName, out ret))
            {
                return ret;
            }
            return null;
        }


        public string GetRandomSpriteName()
        {
            int count = spriteObjArray.Length;
            int index = Random.Range(0, count);
            return spriteObjArray[index].name;
        }

        public int SpriteCount
        {
            get
            {
                return spriteObjArray.Length;
            }
        }

        public Sprite GetSpriteByIndex(int index)
        {
            if (index < 0 || index >= spriteObjArray.Length)
            {
                return null;
            }
            return spriteObjArray[index];
        }

        public string GetSpriteNameByIndex(int index)
        {
            if (index < 0 || index >= spriteObjArray.Length)
            {
                return "";
            }
            return spriteObjArray[index].name;
        }
    }
}
