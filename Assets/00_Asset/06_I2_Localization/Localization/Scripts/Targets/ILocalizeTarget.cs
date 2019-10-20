using UnityEngine;

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{ 
    public abstract class LocalizeTarget : ScriptableObject
    {
        public abstract bool IsValid(Localize cmp);
        public abstract void GetFinalTerms( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm);
        public abstract void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation);

        public abstract bool CanUseSecondaryTerm();
        public abstract bool AllowMainTermToBeRtl();
        public abstract bool AllowSecondTermToBeRtl();
        public abstract ETermType GetPrimaryTermType(Localize cmp);
        public abstract ETermType GetSecondaryTermType(Localize cmp);
    }

    public abstract class LocalizeTarget<T> : LocalizeTarget where T : Object
    {
        public T mTarget;

        public override bool IsValid(Localize cmp)
        {
            if (mTarget!=null)
            {
                var mTargetCmp = mTarget as Component;
                if (mTargetCmp != null && mTargetCmp.gameObject != cmp.gameObject)
                    mTarget = null;
            }
            if (mTarget==null)
                mTarget = cmp.GetComponent<T>();
            return mTarget!=null;
        }
	}
}

