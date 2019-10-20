using _00_Asset._06_I2_Localization.Localization.Scripts.LanguageSource;
using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using UnityEngine;

#pragma warning disable 618

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{
    public class LocalizeTargetDescPrefab : LocalizeTargetDesc<LocalizeTargetUnityStandardPrefab>
    {
        public override bool CanLocalize(Localize cmp) { return true; }
    }

    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityStandardPrefab : LocalizeTarget<GameObject>
    {
        static LocalizeTargetUnityStandardPrefab() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescPrefab() { Name = "Prefab", Priority = 250 }); }

        public override bool IsValid(Localize cmp) { return true; }
        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.GameObject; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Text; }
        public override bool CanUseSecondaryTerm() { return false; }
        public override bool AllowMainTermToBeRtl() { return false; }
        public override bool AllowSecondTermToBeRtl() { return false; }

        public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = cmp.name;
            secondaryTerm = null;
        }

        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            if (string.IsNullOrEmpty(mainTranslation))
                return;

            if (mTarget && mTarget.name == mainTranslation)
                return;

            Transform locTr = cmp.transform;

            var objName = mainTranslation;
            var idx = mainTranslation.LastIndexOfAny(LanguageSourceData.CategorySeparators);
            if (idx >= 0)
                objName = objName.Substring(idx + 1);

            Transform mNew = InstantiateNewPrefab(cmp, mainTranslation);
            if (mNew == null)
                return;
            mNew.name = objName;

            for (int i = locTr.childCount - 1; i >= 0; --i)
            {
                var child = locTr.GetChild(i);
                if (child!=mNew)
                {
                    #if UNITY_EDITOR
                        if (Application.isPlaying)
                            Object.Destroy(child.gameObject);
                        else
                            Object.DestroyImmediate(child.gameObject);
                    #else
				        Object.Destroy (child.gameObject);
                    #endif
                }
            }
        }

        Transform InstantiateNewPrefab(Localize cmp, string mainTranslation)
        {
            GameObject newPrefab = cmp.FindTranslatedObject<GameObject>(mainTranslation);
            if (newPrefab == null)
                return null;

            GameObject current = mTarget as GameObject;

            mTarget = Object.Instantiate(newPrefab);
            if (mTarget == null)
                return null;

            Transform locTr = cmp.transform;
            Transform mNew = (mTarget as GameObject).transform;
            mNew.SetParent(locTr);

            Transform bBase = (current ? current.transform : locTr);
            //mNew.localScale = bBase.localScale;
            mNew.rotation = bBase.rotation;
            mNew.position = bBase.position;

            return mNew;
        }
    }
}
