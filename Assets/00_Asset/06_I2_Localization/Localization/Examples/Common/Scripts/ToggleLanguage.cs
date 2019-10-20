using UnityEngine;
using System.Collections.Generic;

namespace I2.Loc
{
	public class ToggleLanguage : MonoBehaviour 
	{
		void Start () 
		{
			Invoke("Test", 3);
		}

		void Test()
		{
			//--  to move into the next language ----

				List<string> languages = LocalizationManager.GetAllLanguages();
				int index = languages.IndexOf(LocalizationManager.CurrentLanguage);
				if (index<0)
					index = 0;
				else
					index = (index+1) % languages.Count;

			//-- Call this function again in 3 seconds

				Invoke("Test", 3);
		}
	}
}