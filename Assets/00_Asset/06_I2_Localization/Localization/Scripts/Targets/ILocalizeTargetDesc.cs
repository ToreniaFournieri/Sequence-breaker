using System.Collections.Generic;
using UnityEngine;


namespace I2.Loc
{
    public abstract class LocalizeTargetDescriptor
    {
        public string Name;
        public int Priority;
        public abstract bool CanLocalize(Localize cmp);
        public abstract LocalizeTarget CreateTarget(Localize cmp);
        public abstract System.Type GetTargetType();
    }

    public abstract class LocalizeTargetDesc<T> : LocalizeTargetDescriptor where T : LocalizeTarget
    {
        public override LocalizeTarget CreateTarget(Localize cmp) { return ScriptableObject.CreateInstance<T>(); }
        public override System.Type GetTargetType() { return typeof(T); }
    }



    public class LocalizeTargetDescType<T,TG> : LocalizeTargetDesc<TG> where T: Object 
                                                                      where TG: LocalizeTarget<T>
    {
        public override bool CanLocalize(Localize cmp)  { return cmp.GetComponent<T>() != null; }
        public override LocalizeTarget CreateTarget(Localize cmp)
        {
            var target = cmp.GetComponent<T>();
            if (target == null)
                return null;

            var locTarget = ScriptableObject.CreateInstance<TG>();
            locTarget.mTarget = target;
            return locTarget;
        }
    }

}

