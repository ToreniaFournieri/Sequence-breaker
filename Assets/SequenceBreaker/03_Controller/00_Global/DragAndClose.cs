using SequenceBreaker._10_Global;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SequenceBreaker._03_Controller._00_Global
{
    public class DragAndClose : EventTrigger
    {
        private bool _dragging;
        private Transform targetTransform;
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
                targetTransform = gameObject.GetComponent<DragTargetObject>().segueController.GetCurrentView()
                    .transform;         
                
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
        
//            Debug.Log(" mouse pos:" +(Input.mousePosition.x) );
//            Debug.Log(" initial pos:" +(_initialPosition.x ) );

            if (Input.mousePosition.x - ( _initialPosition.x * 2.0/3.0) >= 0  )
            {
//                Debug.Log("proceed");
                // proceed
                _segueController.BackPreviousView();
            }
            else
            {
//                Debug.Log("canceled");
                // quit 
                _segueController.CancelView();
            }
            
        
        }
    }
}
