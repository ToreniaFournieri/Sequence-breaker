using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00_Asset._08_Easy_Panel_Transitions.Demo_Scene.Trailer
{
	public class restart : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
			if(Input.GetKeyDown(KeyCode.Space))
			{
				SceneManager.LoadScene(0);
			}
		
		}
	}
}
