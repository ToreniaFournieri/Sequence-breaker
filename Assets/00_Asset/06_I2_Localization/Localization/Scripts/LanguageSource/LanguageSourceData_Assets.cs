using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace I2.Loc
{
	public partial class LanguageSourceData
	{
        #region Assets

        public void UpdateAssetDictionary()
        {
            assets.RemoveAll(x => x == null);
            MAssetDictionary = assets.Distinct()
                                     .GroupBy(o => o.name)
                                     .ToDictionary(g => g.Key, g => g.First());
        }

        public Object FindAsset( string name )
		{
			if (assets!=null)
			{
                if (MAssetDictionary==null || MAssetDictionary.Count!=assets.Count)
                {
                    UpdateAssetDictionary();
                }
                Object obj;
                if (MAssetDictionary.TryGetValue(name, out obj))
                {
                    return obj;
                }
				//for (int i=0, imax=Assets.Length; i<imax; ++i)
				//	if (Assets[i]!=null && Name.EndsWith( Assets[i].name, StringComparison.OrdinalIgnoreCase))
				//		return Assets[i];
			}
			return null;
		}
		
		public bool HasAsset( Object obj )
		{
			return assets.Contains(obj);
		}

		public void AddAsset( Object obj )
		{
            if (assets.Contains(obj))
                return;

            assets.Add(obj);
            UpdateAssetDictionary();
		}

		
		#endregion
	}
}