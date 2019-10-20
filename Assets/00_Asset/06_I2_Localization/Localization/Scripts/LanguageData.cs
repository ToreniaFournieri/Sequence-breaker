using System;
using UnityEngine.Serialization;

namespace _00_Asset._06_I2_Localization.Localization.Scripts
{
	public enum ELanguageDataFlags
	{
		Disabled = 1,
		KeepLoaded = 2,
		NotLoaded = 4
	}
	[Serializable]
	public class LanguageData
	{
		[FormerlySerializedAs("Name")] public string name;
		[FormerlySerializedAs("Code")] public string code;
		[FormerlySerializedAs("Flags")] public byte flags;      // eLanguageDataFlags

		[NonSerialized]
		public bool Compressed = false;  // This will be used in the next version for only loading used Languages

		public bool IsEnabled () { return (flags & (int)ELanguageDataFlags.Disabled) == 0; }

        public void SetEnabled( bool bEnabled )
        {
            if (bEnabled) flags = (byte)(flags & (~(int)ELanguageDataFlags.Disabled));
                     else flags = (byte)(flags | (int)ELanguageDataFlags.Disabled);
        }

        public bool IsLoaded () { return (flags & (int)ELanguageDataFlags.NotLoaded) == 0; }
		public bool CanBeUnloaded () { return (flags & (int)ELanguageDataFlags.KeepLoaded) == 0; }

		public void SetLoaded ( bool loaded ) 
		{
			if (loaded) flags = (byte)(flags & (~(int)ELanguageDataFlags.NotLoaded));
	  			   else flags = (byte)(flags | (int)ELanguageDataFlags.NotLoaded);
		}
        public void SetCanBeUnLoaded(bool allowUnloading)
        {
            if (allowUnloading) flags = (byte)(flags & (~(int)ELanguageDataFlags.KeepLoaded));
                           else flags = (byte)(flags | (int)ELanguageDataFlags.KeepLoaded);
        }
    }
}