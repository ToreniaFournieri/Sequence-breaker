using System;
using UnityEngine;
using UnityEngine.Events;

namespace SequenceBreaker._03_GUIController.Segue
{
  public class SwipeManager : MonoBehaviour {

    public float swipeThreshold = 50f;
    public float timeThreshold = 0.3f;
    
    public UnityEvent onSwipeLeft;
    public UnityEvent onSwipeRight;
    public UnityEvent onSwipeUp;
    public UnityEvent onSwipeDown;

    
    
    private Vector2 _fingerDown;
    private DateTime _fingerDownTime;
    private Vector2 _fingerUp;
    private DateTime _fingerUpTime;

    private void Update () {
      if (Input.GetMouseButtonDown(0)) {
        _fingerDown = Input.mousePosition;
        _fingerUp = Input.mousePosition;
        _fingerDownTime = DateTime.Now;
      }
      if (Input.GetMouseButtonUp(0)) {
        _fingerDown = Input.mousePosition;
        _fingerUpTime = DateTime.Now;
        CheckSwipe();
      }
      foreach (Touch touch in Input.touches) {
        if (touch.phase == TouchPhase.Began) {
          _fingerDown = touch.position;
          _fingerUp = touch.position;
          _fingerDownTime = DateTime.Now;
        }
        if (touch.phase == TouchPhase.Ended) {
          _fingerDown = touch.position;
          _fingerUpTime = DateTime.Now;
          CheckSwipe();
        }
      }
    }

    private void CheckSwipe() {
      float duration = (float)_fingerUpTime.Subtract(_fingerDownTime).TotalSeconds;
      if (duration > timeThreshold) return;

      float deltaX = _fingerDown.x - _fingerUp.x;
      if (Mathf.Abs(deltaX) > swipeThreshold) {
        if (deltaX > 0) {
          onSwipeRight.Invoke();
          Debug.Log("right");
        } else if (deltaX < 0) {
          onSwipeLeft.Invoke();
          Debug.Log("left");
        }
      }

      float deltaY = _fingerDown.y - _fingerUp.y;
      if (Mathf.Abs(deltaY) > swipeThreshold) {
        if (deltaY > 0) {
          onSwipeUp.Invoke();
          Debug.Log("up");
        } else if (deltaY < 0) {
          onSwipeDown.Invoke();
          Debug.Log("down");
        }
      }

      _fingerUp = _fingerDown;
    }
  }
}