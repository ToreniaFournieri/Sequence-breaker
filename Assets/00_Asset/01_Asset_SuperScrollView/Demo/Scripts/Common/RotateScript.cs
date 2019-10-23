using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
{
    public class RotateScript : MonoBehaviour
    {
        public float speed = 1f;

        // Update is called once per frame
        void Update()
        {
            Vector3 rot = gameObject.transform.localEulerAngles;
            rot.z = rot.z + speed * Time.deltaTime;
            gameObject.transform.localEulerAngles = rot;
        }
    }
}
