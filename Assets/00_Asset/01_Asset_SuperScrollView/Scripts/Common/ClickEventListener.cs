using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperScrollView
{
    public class ClickEventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public static ClickEventListener Get(GameObject obj)
        {
            ClickEventListener listener = obj.GetComponent<ClickEventListener>();
            if (listener == null)
            {
                listener = obj.AddComponent<ClickEventListener>();
            }
            return listener;
        }

        System.Action<GameObject> _mClickedHandler = null;
        System.Action<GameObject> _mDoubleClickedHandler = null;
        System.Action<GameObject> _mOnPointerDownHandler = null;
        System.Action<GameObject> _mOnPointerUpHandler = null;
        bool _mIsPressed = false;

        public bool IsPressd
        {
            get { return _mIsPressed; }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                if (_mDoubleClickedHandler != null)
                {
                    _mDoubleClickedHandler(gameObject);
                }
            }
            else
            {
                if (_mClickedHandler != null)
                {
                    _mClickedHandler(gameObject);
                }
            }

        }
        public void SetClickEventHandler(System.Action<GameObject> handler)
        {
            _mClickedHandler = handler;
        }

        public void SetDoubleClickEventHandler(System.Action<GameObject> handler)
        {
            _mDoubleClickedHandler = handler;
        }

        public void SetPointerDownHandler(System.Action<GameObject> handler)
        {
            _mOnPointerDownHandler = handler;
        }

        public void SetPointerUpHandler(System.Action<GameObject> handler)
        {
            _mOnPointerUpHandler = handler;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            _mIsPressed = true;
            if (_mOnPointerDownHandler != null)
            {
                _mOnPointerDownHandler(gameObject);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _mIsPressed = false;
            if (_mOnPointerUpHandler != null)
            {
                _mOnPointerUpHandler(gameObject);
            }
        }

    }

}
