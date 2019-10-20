using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset._02_Asset_ClassicSRIA.Scripts.Examples.Common
{
    public class DemoUi : MonoBehaviour
	{
		public Button setCountButton;
		public Text countText;
		public Button scrollToButton;
		public Text scrollToText;
		public Button addOneTailButton, removeOneTailButton, addOneHeadButton, removeOneHeadButton;
		public Toggle freezeContentEndEdge;

		public int SetCountValue { get { return int.Parse(countText.text); } }
		public int ScrollToValue { get { return int.Parse(scrollToText.text); } }
	}	
}
