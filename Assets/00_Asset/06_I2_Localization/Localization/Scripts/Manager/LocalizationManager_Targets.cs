using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Collections;

namespace I2.Loc
{
    public static partial class LocalizationManager
    {

        #region Variables: Misc

        public static List<LocalizeTargetDescriptor> MLocalizeTargets = new List<LocalizeTargetDescriptor>();

        #endregion

        public static void RegisterTarget( LocalizeTargetDescriptor desc )
        {
            if (MLocalizeTargets.FindIndex(x => x.Name == desc.Name) != -1)
                return;

            for (int i = 0; i < MLocalizeTargets.Count; ++i)
            {
                if (MLocalizeTargets[i].Priority > desc.Priority)
                {
                    MLocalizeTargets.Insert(i, desc);
                    return;
                }
            }
            MLocalizeTargets.Add(desc);
        }
    }
}
