using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    [SerializeField]　ParticleSystem tapEffect;              // tap effect
    //[SerializeField]　Camera _camera;                        // pos of camera
    public Transform targetTransform;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.mousePosition  + " Input.mousePosition ");
            //var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);

            //tapEffect.transform.position = Input.mousePosition;
            targetTransform.position = Input.mousePosition;
            tapEffect.Emit(1);
        }

    }
}
