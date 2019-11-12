using SequenceBreaker._10_Global;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SequenceBreaker._03_Controller._00_Global
{
    public class DragAndClose : EventTrigger
    {
        private bool _dragging;
        private Transform _targetTransform;
        private Vector3 _initialPosition;
        private SegueController _segueController ;

        private void Start()
        {
            _targetTransform = gameObject.GetComponent<DragTargetObject>().targetView.transform;
            _initialPosition = _targetTransform.position;
            _segueController = gameObject.GetComponent<DragTargetObject>().segueController;
        }

        public void Update() {
            if (_dragging)
            {
                var position = _targetTransform.position;
                position =
                    new Vector2(_initialPosition.x + Input.mousePosition.x, position.y);
                _targetTransform.position = position;
            }
        }
        
        public override void OnPointerDown(PointerEventData eventData) {
            _dragging = true;
        }

        public override void OnPointerUp(PointerEventData eventData) {
            _dragging = false;
        
            Debug.Log(" mouse pos:" +(Input.mousePosition.x) );
            Debug.Log(" initial pos:" +(_initialPosition.x ) );

            if (Input.mousePosition.x - ( _initialPosition.x * 2.0/3.0) >= 0  )
            {
                // proceed
                _segueController.BackPreviousView();
            }
            else
            {
                // quit 
                _segueController.CancelView();
            }
            
        
        }
    }
}
