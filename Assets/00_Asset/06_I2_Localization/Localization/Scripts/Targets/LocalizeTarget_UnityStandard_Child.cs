using UnityEngine;

namespace I2.Loc
{
    public class LocalizeTargetDescChild : LocalizeTargetDesc<LocalizeTargetUnityStandardChild>
    {
        public override bool CanLocalize(Localize cmp) { return cmp.transform.childCount > 1; }
    }

    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityStandardChild : LocalizeTarget<GameObject>
    {
        static LocalizeTargetUnityStandardChild() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescChild() { Name = "Child", Priority = 200 }); }

        public override bool IsValid(Localize cmp) { return cmp.transform.childCount>1; }
        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.GameObject; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Text; }
        public override bool CanUseSecondaryTerm() { return false; }
        public override bool AllowMainTermToBeRtl() { return false; }
        public override bool AllowSecondTermToBeRtl() { return false; }

        public override void GetFinalTerms(Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = cmp.name;
            secondaryTerm = null;
        }

        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            if (string.IsNullOrEmpty(mainTranslation))
                return;
            Transform locTr = cmp.transform;

            var objName = mainTranslation;
            var idx = mainTranslation.LastIndexOfAny(LanguageSourceData.CategorySeparators);
            if (idx >= 0)
                objName = objName.Substring(idx + 1);

            for (int i = 0; i < locTr.childCount; ++i)
            {
                var child = locTr.GetChild(i);
                child.gameObject.SetActive(child.name == objName);
            }
        }
    }
}