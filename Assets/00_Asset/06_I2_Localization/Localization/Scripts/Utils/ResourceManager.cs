using System.Collections.Generic;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if UNITY_5_4_OR_NEWER

#endif

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Utils
{
	public interface IResourceManagerBundles
	{
		Object LoadFromBundle(string path, System.Type assetType );
	}

	public class ResourceManager : MonoBehaviour 
	{
		#region Singleton
		public static ResourceManager PInstance
		{
			get {
				bool changed = _mInstance==null;

				if (_mInstance==null)
					_mInstance = (ResourceManager)FindObjectOfType(typeof(ResourceManager));

				if (_mInstance==null)
				{
					GameObject go = new GameObject("I2ResourceManager", typeof(ResourceManager));
					go.hideFlags = go.hideFlags | HideFlags.HideAndDontSave;	// Only hide it if this manager was autocreated
					_mInstance = go.GetComponent<ResourceManager>();
					#if UNITY_5_4_OR_NEWER
					SceneManager.sceneLoaded += MyOnLevelWasLoaded;
					#endif
				}

				if (changed && Application.isPlaying)
					DontDestroyOnLoad(_mInstance.gameObject);

				return _mInstance;
			}
		}
		static ResourceManager _mInstance;

		#endregion

		#region Management

		public List<IResourceManagerBundles> mBundleManagers = new List<IResourceManagerBundles>();

		#if UNITY_5_4_OR_NEWER
		public static void MyOnLevelWasLoaded(Scene scene, LoadSceneMode mode)
		#else
		public void OnLevelWasLoaded()
		#endif
		{
			PInstance.CleanResourceCache();
			LocalizationManager.UpdateSources();
		}

		#endregion

		#region Assets

		[FormerlySerializedAs("Assets")] public Object[] assets;

		// This function tries finding an asset in the Assets array, if not found it tries loading it from the Resources Folder
		public T GetAsset<T>( string name ) where T : Object
		{
			T obj = FindAsset( name ) as T;
			if (obj!=null)
				return obj;

			return LoadFromResources<T>( name );
		}

		Object FindAsset( string name )
		{
			if (assets!=null)
			{
				for (int i=0, imax=assets.Length; i<imax; ++i)
					if (assets[i]!=null && assets[i].name == name)
						return assets[i];
			}
			return null;
		}

		public bool HasAsset( Object obj )
		{
			if (assets==null)
				return false;
			return System.Array.IndexOf (assets, obj) >= 0;
		}

		#endregion

		#region Resources Cache

		// This cache is kept for a few moments and then cleared
		// Its meant to avoid doing several Resource.Load for the same Asset while Localizing 
		// (e.g. Lot of labels could be trying to Load the same Font)
		readonly Dictionary<string, Object> _mResourcesCache = new Dictionary<string, Object>(System.StringComparer.Ordinal); // This is used to avoid re-loading the same object from resources in the same frame
		//bool mCleaningScheduled = false;

		public T LoadFromResources<T>( string path ) where T : Object
		{
			try
			{
				if (string.IsNullOrEmpty( path ))
					return null;

				Object obj;
				// Doing Resource.Load is very slow so we are catching the recently loaded objects
				if (_mResourcesCache.TryGetValue( path, out obj ) && obj!=null)
				{
					return obj as T;
				}

				obj = null;

                if (path.EndsWith("]", System.StringComparison.OrdinalIgnoreCase))  // Handle sprites (Multiple) loaded from resources :   "SpritePath[SpriteName]"
                {
                    int idx = path.LastIndexOf("[", System.StringComparison.OrdinalIgnoreCase);
                    int len = path.Length - idx - 2;
                    string multiSpriteName = path.Substring(idx + 1, len);
                    path = path.Substring(0, idx);

                    T[] objs = Resources.LoadAll<T>(path);
                    for (int j = 0, jmax = objs.Length; j < jmax; ++j)
                        if (objs[j].name.Equals(multiSpriteName))
                        {
                            obj = objs[j];
                            break;
                        }
                }
                else
                {
                    obj = Resources.Load(path, typeof(T)) as T;
                }

				if (obj == null)
					obj = LoadFromBundle<T>( path );

				if (obj!=null)
					_mResourcesCache[path] = obj;

				/*if (!mCleaningScheduled)
				{
					Invoke("CleanResourceCache", 0.1f);
					mCleaningScheduled = true;
				}*/
				//if (obj==null)
					//Debug.LogWarningFormat( "Unable to load {0} '{1}'", typeof( T ), Path );

				return obj as T;
			}
			catch (System.Exception e)
			{
				Debug.LogErrorFormat( "Unable to load {0} '{1}'\nERROR: {2}", typeof(T), path, e.ToString() );
				return null;
			}
		}

		public T LoadFromBundle<T>(string path ) where T : Object
		{
			for (int i = 0, imax = mBundleManagers.Count; i < imax; ++i)
				if (mBundleManagers[i]!=null)
				{
					var obj = mBundleManagers[i].LoadFromBundle(path, typeof(T)) as T;
					if (obj != null)
						return obj;
				}
			return null;
		}

		public void CleanResourceCache()
		{
			_mResourcesCache.Clear();
			Resources.UnloadUnusedAssets();

			CancelInvoke();
			//mCleaningScheduled = false;
		}

		#endregion
	}
}