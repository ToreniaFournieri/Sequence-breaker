using System;
using System.Collections.Generic;
using SequenceBreaker.Master.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._11_UnitSet
{
    public class UnitSetExcelConverter : EditorWindow
    {
        public UnitSetExcelImport unitSetExcelImport;
        public UnitSet unitSet;

        private static string _excelPath = "UnitSetExcel";
//        private static string _targetPath = "UnitSetObjectPath";

        private string _targetPathWithoutName;
           
        [MenuItem("Window/Unit Set Excel Converter %#s")]
        static void Init()
        {
            GetWindow(typeof(UnitSetExcelConverter));
        }

        private void OnEnable()
        {
            if(EditorPrefs.HasKey(_excelPath)) 
            {
                string excelPath = EditorPrefs.GetString(_excelPath);
                unitSetExcelImport = AssetDatabase.LoadAssetAtPath (excelPath, typeof(UnitSetExcelImport)) as UnitSetExcelImport;
            }
//            if(EditorPrefs.HasKey(_targetPath)) 
//            {
//                string objectPath = EditorPrefs.GetString(_targetPath);
//                unitSet = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnitSet)) as UnitSet;
//            }
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

            if (GUILayout.Button("2. Select [Export] Unit Set path", GUILayout.ExpandWidth(false)))
            {
                OpenTargetList();
            }

            GUILayout.Label(_targetPathWithoutName ?? " Unselected");

            GUILayout.EndHorizontal();

                    
            if (GUILayout.Button("3. Convert Excel to Unit Set", GUILayout.ExpandWidth(false)))
            {
                ConvertUnitSetFromExcel();
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

            string absPath = EditorUtility.OpenFolderPanel ("Select Target List Path", "", "");
            if (absPath.StartsWith(Application.dataPath)) 
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                _targetPathWithoutName = relPath;
//                Debug.Log(_targetPathWithoutName);
//                unitSet = AssetDatabase.LoadAssetAtPath (relPath, typeof(UnitSet)) as UnitSet;
//                if (unitSet != null && unitSet.unitSetList == null)
//                    unitSet.unitSetList = new List<List<UnitClass>>();
//                if (unitSet) {
//                    EditorPrefs.SetString(_targetPath, relPath);
//                }
            }
        }

        void ConvertUnitSetFromExcel()
        {

            int currentMissionId = 0;
                foreach (var unitMasterExcel in unitSetExcelImport.unitSetExcelList)
                {
                    
                    if (unitMasterExcel.missionId != currentMissionId)
                    {
                        //new mission start. so creat it.
                        currentMissionId = unitMasterExcel.missionId;
                        
//                        string _unitSetPath =  _targetPathWithoutName + "/UnitSet-" + currentMissionId + ".asset";
                        

                        string _unitSetPath =  _targetPathWithoutName + "/UnitSet-" + currentMissionId ;

                            //load scriptable object from Resources need ref path.
                            var q = "/Resources/";
                            var index = _unitSetPath.IndexOf(q, StringComparison.Ordinal) + q.Length;
                            string _refUnitSetPath = _unitSetPath.Substring(index);
                            

                        UnitSet unitSetCheck = Resources.Load<UnitSet>(_refUnitSetPath);
                        if (unitSetCheck == null)
                        {
//                            Debug.Log("not exist missionId: " + currentMissionId + " Path:" + _refUnitSetPath );
                            unitSet = UnitSetCreate.Create(_unitSetPath + ".asset");
                        }
                        else
                        {
                            unitSet = unitSetCheck;
                        }

                        unitSet.unitSetList = new List<Runbattle>();

                        unitSet.missionId = unitMasterExcel.missionId;
                    }

                    var unitWavePath = _targetPathWithoutName + "/UnitWave/" + currentMissionId + "-" + unitMasterExcel.waveId + ".asset";

                    Runbattle unitWave = unitMasterExcel.GetUnitSet(unitWavePath);
                    if (unitWave.unitWave != null)
                    {
                        unitSet.unitSetList.Add(unitWave);
                    }
                }
            
            
            
        }

    }
}
