using System;
using SequenceBreaker._04_Timeline_Tab.Log;
using UnityEngine;
using UnityEngine.Events;

namespace SequenceBreaker._10_Global
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
        this._fingerDown = Input.mousePosition;
        this._fingerUp = Input.mousePosition;
        this._fingerDownTime = DateTime.Now;
      }
      if (Input.GetMouseButtonUp(0)) {
        this._fingerDown = Input.mousePosition;
        this._fingerUpTime = DateTime.Now;
        this.CheckSwipe();
      }
      foreach (Touch touch in Input.touches) {
        if (touch.phase == TouchPhase.Began) {
          this._fingerDown = touch.position;
          this._fingerUp = touch.position;
          this._fingerDownTime = DateTime.Now;
        }
        if (touch.phase == TouchPhase.Ended) {
          this._fingerDown = touch.position;
          this._fingerUpTime = DateTime.Now;
          this.CheckSwipe();
        }
      }
    }

    private void CheckSwipe() {
      float duration = (float)this._fingerUpTime.Subtract(this._fingerDownTime).TotalSeconds;
      if (duration > this.timeThreshold) return;

      float deltaX = this._fingerDown.x - this._fingerUp.x;
      if (Mathf.Abs(deltaX) > this.swipeThreshold) {
        if (deltaX > 0) {
          this.onSwipeRight.Invoke();
          Debug.Log("right");
        } else if (deltaX < 0) {
          this.onSwipeLeft.Invoke();
          Debug.Log("left");
        }
      }

      float deltaY = _fingerDown.y - _fingerUp.y;
      if (Mathf.Abs(deltaY) > this.swipeThreshold) {
        if (deltaY > 0) {
          this.onSwipeUp.Invoke();
          Debug.Log("up");
        } else if (deltaY < 0) {
          this.onSwipeDown.Invoke();
          Debug.Log("down");
        }
      }

      this._fingerUp = this._fingerDown;
    }
  }
}