using System.Collections.Generic;
using SequenceBreaker._01_Data.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class UnitMasterExcelConverter : EditorWindow
    {
        public UnitMasterExcelImport unitMasterExcelImport;
        public UnitMasterList unitMasterList;
        
        private int viewIndex = 1;

        private static string _excelPath = "ExcelPath";
        private static string _targetPath = "ObjectPath";

        
        [MenuItem("Window/Unit Master Excel Converter %#e")]
        static void Init()
        {
            GetWindow(typeof(UnitMasterExcelConverter));
        }

        private void OnEnable()
        {
            if(EditorPrefs.HasKey(_excelPath)) 
            {
                string excelPath = EditorPrefs.GetString(_excelPath);
                unitMasterExcelImport = AssetDatabase.LoadAssetAtPath (excelPath, typeof(UnitMasterExcelImport)) as UnitMasterExcelImport;
            }
            if(EditorPrefs.HasKey(_targetPath)) 
            {
                string objectPath = EditorPrefs.GetString(_targetPath);
                unitMasterList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnitMasterList)) as UnitMasterList;
            }
        }

        private void OnGUI()
        {
            
                GUILayout.Label("Unit Master Excel Converter", EditorStyles.boldLabel);

                
                GUILayout.Space(20);

            
                    GUILayout.Space(10);
                    
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("1. Select [Import] Unit Master Excel asset", GUILayout.ExpandWidth(false)))
                    {
                        OpenUnitMasterExcel();
                    }

                    GUILayout.Label(unitMasterExcelImport ? unitMasterExcelImport.name : " Unselected");

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("2. Select [Export] Unit master asset", GUILayout.ExpandWidth(false)))
                    {
                        OpenItemList();
                    }

                    GUILayout.Label(unitMasterList ? unitMasterList.name : " Unselected");

                    GUILayout.EndHorizontal();

                    
                    if (GUILayout.Button("3. Convert Excel to Unit List", GUILayout.ExpandWidth(false)))
                    {
                        ConvertUnitListFromExcel();
                    }
                    GUILayout.Space(20);
        }
        
        void OpenUnitMasterExcel () 
        {
            string absPath = EditorUtility.OpenFilePanel ("Select Unit Master Excel", "", "");
            if (absPath.StartsWith(Application.dataPath)) 
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitMasterExcelImport = AssetDatabase.LoadAssetAtPath (relPath, typeof(UnitMasterExcelImport)) as UnitMasterExcelImport;
                if (unitMasterExcelImport.unitMasterExcel == null)
                    unitMasterExcelImport.unitMasterExcel = new List<UnitMasterExcel>();
                if (unitMasterExcelImport) {
                    EditorPrefs.SetString(_excelPath, relPath);
                }
            }
        }
        void OpenItemList () 
        {
            string absPath = EditorUtility.OpenFilePanel ("Select Unit List", "", "");
            if (absPath.StartsWith(Application.dataPath)) 
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitMasterList = AssetDatabase.LoadAssetAtPath (relPath, typeof(UnitMasterList)) as UnitMasterList;
                if (unitMasterList.unitList == null)
                    unitMasterList.unitList = new List<UnitMaster>();
                if (unitMasterList) {
                    EditorPrefs.SetString(_targetPath, relPath);
                }
            }
        }
        
        void ConvertUnitListFromExcel () 
        {
            // There is no overwrite protection here!
            // There is No "Are you sure you want to overwrite your existing object?" if it exists.
            // This should probably get a string from the user to create a new unitName and pass it ...
            viewIndex = 1;
            
            string objectPath = EditorPrefs.GetString(_targetPath);

            unitMasterList = UnitMasterListCreate.Create(objectPath);
            if (unitMasterList) 
            {
                unitMasterList.unitList = new List<UnitMaster>();
                foreach (UnitMasterExcel unitMasterExcel in unitMasterExcelImport.unitMasterExcel)
                {
                    unitMasterList.unitList.Add(unitMasterExcel.GetUnitMaster());
                }

            }
            


        }
    }
}
