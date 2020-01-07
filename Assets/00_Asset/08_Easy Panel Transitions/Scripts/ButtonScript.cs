using UnityEngine;
using UnityEngine.Events;

namespace _00_Asset._08_Easy_Panel_Transitions.Scripts
{
    public class ButtonScript : MonoBehaviour {
    
        public UnityEvent onInteract = new UnityEvent();

        public void OnClick()
        {
            // Trigger the event
            onInteract.Invoke();
        }
    }
}
