using _00_Asset._06_I2_Localization.Localization.Scripts.Manager;
using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;
using UnityEngine;

#pragma warning disable 618

namespace _00_Asset._06_I2_Localization.Localization.Scripts.Targets
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
    #endif

    public class LocalizeTargetUnityStandardMeshRenderer : LocalizeTarget<MeshRenderer>
    {
        static LocalizeTargetUnityStandardMeshRenderer() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDescType<MeshRenderer, LocalizeTargetUnityStandardMeshRenderer>() { Name = "MeshRenderer", Priority = 800 }); }

        public override ETermType GetPrimaryTermType(Localize cmp) { return ETermType.Mesh; }
        public override ETermType GetSecondaryTermType(Localize cmp) { return ETermType.Material; }
        public override bool CanUseSecondaryTerm() { return true; }
        public override bool AllowMainTermToBeRtl() { return false; }
        public override bool AllowSecondTermToBeRtl() { return false; }

        public override void GetFinalTerms ( Localize cmp, string main, string secondary, out string primaryTerm, out string secondaryTerm)
        {
            if (mTarget==null)
            {
                primaryTerm = secondaryTerm = null;
            }
            else
            {
                MeshFilter filter = mTarget.GetComponent<MeshFilter>();
                if (filter==null || filter.sharedMesh==null)
                {
                    primaryTerm = null;
                }
                else
                {
                    #if UNITY_EDITOR
                        primaryTerm = UnityEditor.AssetDatabase.GetAssetPath(filter.sharedMesh);
                        I2Utils.RemoveResourcesPath(ref primaryTerm);
                    #else
                        primaryTerm = filter.sharedMesh.name;
                    #endif
                }
            }

            if (mTarget==null || mTarget.sharedMaterial==null)
            {
                secondaryTerm = null;
            }
            else
            {
                #if UNITY_EDITOR
                    secondaryTerm = UnityEditor.AssetDatabase.GetAssetPath(mTarget.sharedMaterial);
                    I2Utils.RemoveResourcesPath(ref secondaryTerm);
                #else
                    secondaryTerm = mTarget.sharedMaterial.name;
                #endif
            }
        }

        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            //--[ Localize Material]----------
            Material newMat = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
            if (newMat != null && mTarget.sharedMaterial != newMat)
            {
                mTarget.material = newMat;
            }

            //--[ Localize Mesh ]----------
            Mesh newMesh = cmp.FindTranslatedObject<Mesh>( mainTranslation);
            MeshFilter filter = mTarget.GetComponent<MeshFilter>();
            if (newMesh != null && filter.sharedMesh != newMesh)
            {
                filter.mesh = newMesh;
            }
        }
    }
}