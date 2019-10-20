using System.Collections.Generic;
using _00_Asset._06_I2_Localization.Localization.Scripts.Targets;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Manager
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
