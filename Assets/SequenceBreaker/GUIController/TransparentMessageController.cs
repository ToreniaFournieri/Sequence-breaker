using System.Collections.Generic;
using SequenceBreaker.Environment;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.GUIController
{
    public sealed class TransparentMessageController : MonoBehaviour
    {
        public GameObject transparentMessage;
        public SimpleObjectPool transparentObjectPool;
        public List<(string message, long timestamp)> transparentStringDateList;

        private long deleteTime = 50000000; // 5seconds

        private float timeLeft;

        void Start()
        {
            transparentStringDateList = new List<(string message, long timestamp)>();
        }

        public void CloseMessage()
        {
            transparentStringDateList.Clear();
            //transparentText.text = null;
            transparentMessage.SetActive(false);

        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0)
            {
                timeLeft = 1.0f;


                refreshMessage();

            }

        }

        private void refreshMessage()
        {
            long currentTime = System.DateTime.Now.ToBinary();

            Debug.Log("1 seconds came :" + transparentStringDateList.Count);

            bool isDeleteHappen = false;
            for (int i = transparentStringDateList.Count - 1 ; i >= 0 ; i--)
            {
                Debug.Log((currentTime - transparentStringDateList[i].timestamp) + " second? " +currentTime + " vs" + transparentStringDateList[i].timestamp);

                if ( currentTime - transparentStringDateList[i].timestamp >= deleteTime)
                {
                    Debug.Log("5 seconds came :" + transparentStringDateList[i].message);

                    transparentStringDateList.Remove(transparentStringDateList[i]);

                    isDeleteHappen = true;
                }
            }

            if (isDeleteHappen)
            {
              GameObject[] gameObjects = transparentMessage.transform.GetComponentsInChildren<GameObject>();
                foreach (GameObject gameobject in gameObjects)
                {
                    gameobject.GetComponent<Text>().text = null;

                    Debug.Log("will destroy :" + gameobject.name);
                    Destroy(gameObject);
                }

            }

        }

        public void AddTextAndActive(string message)
        {
            if (transparentMessage.activeInHierarchy == false)
            {
                transparentStringDateList.Clear();
            }

            (string, long) _transparentStringDate = (message, System.DateTime.Now.ToBinary());
            transparentStringDateList.Add(_transparentStringDate);

            var textObject = transparentObjectPool.GetObject();

            //textObject.GetComponent<RectTransform>().SetParent(transparentMessage.GetComponent<RectTransform>());
            textObject.transform.SetParent(transparentMessage.GetComponent<RectTransform>());

            //textObject.transform.
            transparentMessage.SetActive(true);


        }
    }
}
