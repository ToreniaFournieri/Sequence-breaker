using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _00_Asset._02_Asset_ClassicSRIA.Scripts.Util
{
    /// <summary>
	/// Utility to expand an item when it's clicked, dispatching the size change request via <see cref="ISizeChangesHandler"/> for increased flexibility. 
	/// This is a duplicate of ExpandCollapseOnClick from the Optimized ScrollView Adapter package
	/// </summary>
    public class CExpandCollapseOnClick : MonoBehaviour
    {
		/// <summary>
		/// The button to whose onClock to subscribe. If not specified, will try to GetComponent&lt;Button&gt; from the GO containing this script 
		/// </summary>
		[Tooltip("will be taken from this object, if not specified")]
        public Button button;

        /// <summary>When expanding, the initial size will be <see cref="nonExpandedSize"/> and the target size will be <see cref="nonExpandedSize"/> x <see cref="ExpandFactor"/>; opposite is true when collapsing</summary>
		[NonSerialized] // must be set through code
        public float ExpandFactor = 2f;

        /// <summary>The duration of the expand(or collapse) animation</summary>
        public float animDuration = .2f;

        /// <summary>This is the size from which the item will start expanding</summary>
        [HideInInspector]
        public float nonExpandedSize = -1f;

        /// <summary>This keeps track of the 'expanded' state. If true, on click the animation will set <see cref="nonExpandedSize"/> as the target size; else, <see cref="nonExpandedSize"/> x <see cref="ExpandFactor"/> </summary>
        [HideInInspector]
        public bool expanded;

		public UnityFloatEvent onExpandAmounChanged;

		float _startSize;
        float _endSize;
        float _animStart;
        //float animEnd;
        bool _animating = false;
        RectTransform _rectTransform;

        public ISizeChangesHandler sizeChangesHandler;


        void Awake()
        {
            _rectTransform = transform as RectTransform;

            if (button == null)
                button = GetComponent<Button>();

			if (button)
				button.onClick.AddListener(OnClicked);
        }

        public void OnClicked()
        {
            if (_animating)
                return;

            if (nonExpandedSize < 0f)
                return;

            _animating = true;
            _animStart = Time.time;
            //animEnd = animStart + animDuration;

            if (expanded) // shrinking
            {
                _startSize = nonExpandedSize * ExpandFactor;
                _endSize = nonExpandedSize;
            }
            else // expanding
            {
                _startSize = nonExpandedSize;
                _endSize = nonExpandedSize * ExpandFactor;
            }
        }


        void Update()
        {
            if (_animating)
            {
                float elapsedTime = Time.time - _animStart;
                float t01 = elapsedTime / animDuration;
				if (t01 >= 1f) // done
				{
					t01 = 1f; // fill/clamp animation
					_animating = false;
				}
				else
					t01 = Mathf.Sqrt(t01); // fast-in, slow-out effect

				float size = Mathf.Lerp(_startSize, _endSize, t01);
                if (sizeChangesHandler == null)
				{
					//// debug
					//rectTransform.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(RectTransform.Edge.Top, rectTransform.GetInsetFromParentTopEdge(rectTransform.parent as RectTransform), size);
				}
				else
                {
                    bool accepted = sizeChangesHandler.HandleSizeChangeRequest(_rectTransform, size);

                    // Interruption
                    if (!accepted)
                        _animating = false;

                    if (!_animating) // done; even if it wasn't accepted, wether we should or shouldn't change the "expanded" state depends on the user's requirements. We chose to change it
                    {
                        expanded = !expanded;
                        sizeChangesHandler.OnExpandedStateChanged(_rectTransform, expanded);
					}
				}


				if (onExpandAmounChanged != null)
					onExpandAmounChanged.Invoke(t01);
			}
        }

        /// <summary>Interface to implement by the class that'll handle the size changes when the animation runs</summary>
        public interface ISizeChangesHandler
        {
            /// <summary>Called each frame during animation</summary>
            /// <param unitName="rt">The animated RectTransform</param>
            /// <param unitName="newSize">The requested size</param>
            /// <returns>If it was accepted</returns>
            bool HandleSizeChangeRequest(RectTransform rt, float newSize);

            /// <summary>Called when the animation ends and the item successfully expanded (<paramref unitName="expanded"/> is true) or collapsed (else)</summary>
            /// <param unitName="rt">The animated RectTransform</param>
            /// <param unitName="expanded">true if the item expanded. false, if collapsed</param>
            void OnExpandedStateChanged(RectTransform rt, bool expanded);
        }

		[Serializable]
		public class UnityFloatEvent : UnityEvent<float> { }
    }
}