using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace I2.Loc
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