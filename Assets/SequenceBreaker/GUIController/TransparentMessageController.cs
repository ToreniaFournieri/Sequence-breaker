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

        private long deleteTime = 80000000; // 5seconds



        private float timeLeft;

        void Start()
        {
            transparentStringDateList = new List<(string message, long timestamp)>();
        }

        public void CloseMessage()
        {
            refreshMessage(true);
            transparentMessage.SetActive(false);

        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0)
            {
                timeLeft = 1.0f;


                refreshMessage(false);

            }

        }

        private void refreshMessage(bool ignoreTimeDelta)
        {
            long currentTime = System.DateTime.Now.ToBinary();


            for (int i = transparentStringDateList.Count - 1 ; i >= 0 ; i--)
            {
                if (ignoreTimeDelta)
                {
                    transparentStringDateList.Remove(transparentStringDateList[i]);
                    ReturnGameObject(transparentMessage.transform.GetChild(i).gameObject);
                    continue;
                }


                if ( currentTime - transparentStringDateList[i].timestamp >= deleteTime)
                {
                    transparentStringDateList.Remove(transparentStringDateList[i]);
                    ReturnGameObject(transparentMessage.transform.GetChild(i).gameObject);
                }
            }


        }

        private void ReturnGameObject(GameObject returnObject)
        {
            if (returnObject)
            {
                transparentObjectPool.ReturnObject(returnObject);
            }
        }

        public void AddTextAndActive(string message, bool letBoldRed)
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

            textObject.GetComponentInChildren<Text>().text = message;

            if (letBoldRed)
            {
                textObject.GetComponentInChildren<Text>().color = Color.red;

                Color backgroundColor = new Color(1f,1f,1f,0.7f);
                textObject.GetComponent<Image>().color = backgroundColor;

            } else
            {
                textObject.GetComponentInChildren<Text>().color = Color.yellow;

                Color backgroundColor = new Color(0f, 0f, 0f, 0.7f);
                textObject.GetComponent<Image>().color = backgroundColor;

            }

            Vector3 vector = new Vector3(1, 1, 1);
            textObject.transform.localScale = vector;
            transparentMessage.SetActive(true);


        }
    }
}
