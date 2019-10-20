using System.Collections;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{
	// This class is used to spawn coroutines from outside of MonoBehaviors
	public class CoroutineManager : MonoBehaviour 
	{
		static CoroutineManager PInstance
		{
			get{
				if (_mInstance==null)
				{
					GameObject go = new GameObject( "_Coroutiner" );
                    go.hideFlags = HideFlags.HideAndDontSave;
                    _mInstance = go.AddComponent<CoroutineManager>();
                    if (Application.isPlaying)
                        DontDestroyOnLoad(go);
                }
                return _mInstance;
			}
		}
        static CoroutineManager _mInstance;


        private void Awake()
        {
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }

        public static Coroutine Start(IEnumerator coroutine)
		{
			#if UNITY_EDITOR
				// Special case to allow coroutines to run in the Editor
				if (!Application.isPlaying)
				{
					UnityEditor.EditorApplication.CallbackFunction delg=null;
					delg = delegate () 
					{
						if (!coroutine.MoveNext())
							UnityEditor.EditorApplication.update -= delg;
					};
					UnityEditor.EditorApplication.update += delg;
					return null;
				}
			#endif

			return PInstance.StartCoroutine(coroutine);
		}
	}
}
