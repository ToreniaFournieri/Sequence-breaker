

namespace _00_Asset._02_Asset_ClassicSRIA.Scripts.Examples.Common
{
	public static class CUtil
	{

		// Utility randomness methods
		public static int Rand(int maxExcl) { return UnityEngine.Random.Range(0, maxExcl); }
		public static float RandF(float max = 1f) { return UnityEngine.Random.Range(0, max); }
	}
}
