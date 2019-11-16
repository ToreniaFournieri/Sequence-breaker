using System.Collections.Generic;
using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class UnitSetExcelConverter : EditorWindow
    {
        public UnitSetExcelImport unitSetExcelImport;
        public UnitSet unitSet;

        private static string _excelPath = "UnitSetExcel";
        private static string _targetPath = "UnitSetObjectPath";

           
        [MenuItem("Window/Unit Set Excel Converter %#s")]
        static void Init()
        {
            GetWindow(typeof(UnitMasterExcelConverter));
        }

        private void OnEnable()
        {
            if(EditorPrefs.HasKey(_excelPath)) 
            {
                string excelPath = EditorPrefs.GetString(_excelPath);
                unitSetExcelImport = AssetDatabase.LoadAssetAtPath (excelPath, typeof(UnitMasterExcelImport)) as UnitSetExcelImport;
            }
            if(EditorPrefs.HasKey(_targetPath)) 
            {
                string objectPath = EditorPrefs.GetString(_targetPath);
                unitSet = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnitMasterList)) as UnitSet;
            }
        }
        
        private void OnGUI()
        {
            
            GUILayout.Label("Unit Set Excel Converter", EditorStyles.boldLabel);

                
            GUILayout.Space(20);

            
            GUILayout.Space(10);
                    
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1. Select [Import] Unit Set Excel asset", GUILayout.ExpandWidth(false)))
            {
                OpenUnitMasterExcel();
            }

            GUILayout.Label(unitSetExcelImport ? unitSetExcelImport.name : " Unselected");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("2. Select [Export] Unit Set asset", GUILayout.ExpandWidth(false)))
            {
                OpenTargetList();
            }

            GUILayout.Label(unitSet ? unitSet.name : " Unselected");

            GUILayout.EndHorizontal();

                    
            if (GUILayout.Button("3. Convert Excel to Unit Set", GUILayout.ExpandWidth(false)))
            {
//                ConvertUnitListFromExcel();
            }
            GUILayout.Space(20);
        }

        void OpenUnitMasterExcel () 
        {
            string absPath = EditorUtility.OpenFilePanel ("Select Unit Set Excel", "", "");
            if (absPath.StartsWith(Application.dataPath)) 
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitSetExcelImport = AssetDatabase.LoadAssetAtPath (relPath, typeof(UnitSetExcelImport)) as UnitSetExcelImport;
                if (unitSetExcelImport != null && unitSetExcelImport.unitSetExcelList == null)
                    unitSetExcelImport.unitSetExcelList = new List<UnitSetExcel>();
                if (unitSetExcelImport) {
                    EditorPrefs.SetString(_excelPath, relPath);
                }
            }
        }
        
        void OpenTargetList () 
        {
            string absPath = EditorUtility.OpenFilePanel ("Select Target List", "", "");
            if (absPath.StartsWith(Application.dataPath)) 
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitSet = AssetDatabase.LoadAssetAtPath (relPath, typeof(UnitSet)) as UnitSet;
                if (unitSet != null && unitSet.unitSetList == null)
                    unitSet.unitSetList = new List<List<UnitClass>>();
                if (unitSet) {
                    EditorPrefs.SetString(_targetPath, relPath);
                }
            }
        }

    }
}
