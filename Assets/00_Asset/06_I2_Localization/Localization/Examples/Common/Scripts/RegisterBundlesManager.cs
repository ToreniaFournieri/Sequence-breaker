using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;
using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Examples.Common.Scripts
{
    public class RegisterBundlesManager : MonoBehaviour, IResourceManagerBundles
	{
		public void OnEnable()
		{
            if (!ResourceManager.PInstance.mBundleManagers.Contains(this))
            {
                ResourceManager.PInstance.mBundleManagers.Add(this);
            }
		}

        public void OnDisable()
        {
            ResourceManager.PInstance.mBundleManagers.Remove(this);
        }

        public virtual Object LoadFromBundle(string path, System.Type assetType)
        {
            // load from a bundle using path and return the object
            return null;
        }
}


    // To use bundles, create a class similar to this one
    // and add it to one of your scenes
    //
    
    /*public class CustomBundlesManager : RegisterBundlesManager
    {
        public override Object LoadFromBundle(string path, System.Type assetType )
        {
            // load from a bundle using path and return the object
            return null;
        }
    }
    */
}