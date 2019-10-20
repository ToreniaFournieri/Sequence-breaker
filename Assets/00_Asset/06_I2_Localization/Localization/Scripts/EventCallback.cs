using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _00_Asset._06_I2_Localization.Localization.Scripts
{
	[Serializable]
	public class EventCallback
	{
		[FormerlySerializedAs("Target")] public MonoBehaviour target;
		[FormerlySerializedAs("MethodName")] public string methodName = string.Empty;

		public void Execute( UnityEngine.Object sender = null )
		{
			if (HasCallback() && Application.isPlaying)
				target.gameObject.SendMessage(methodName, sender, SendMessageOptions.DontRequireReceiver);
		}

		public bool HasCallback()
		{
			return target != null && !string.IsNullOrEmpty (methodName);
		}
	}
}