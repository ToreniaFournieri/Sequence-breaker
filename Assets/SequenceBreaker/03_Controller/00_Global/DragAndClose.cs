using SequenceBreaker._10_Global;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SequenceBreaker._03_Controller._00_Global
{
    public class DragAndClose : EventTrigger
    {
        public Transform targetTransform;
        private bool _dragging;
        private Vector3 _initialPosition;
        private SegueController _segueController ;

        private void Start()
        {
            Init();
        }


        public void Init()
        {
            targetTransform = gameObject.GetComponent<DragTargetObject>().segueController.GetCurrentView()
                    .transform;
            _initialPosition = targetTransform.position;
                _segueController = gameObject.GetComponent<DragTargetObject>().segueController;
        }

        public void Update() {
            if (_dragging)
            {
                var position = targetTransform.position;
                position =
                    new Vector2(_initialPosition.x + Input.mousePosition.x, position.y);
                targetTransform.position = position;
            }
        }
        
        public override void OnPointerDown(PointerEventData eventData) {
            _dragging = true;
        }

        public override void OnPointerUp(PointerEventData eventData) {
            _dragging = false;
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
