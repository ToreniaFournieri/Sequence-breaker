using System;
using System.Collections.Generic;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.UnitClass;
using SequenceBreaker.Play.Prepare;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._11_UnitSet
{

    public class UnitSetExcelConverter : EditorWindow
    {
        public UnitSetExcelImport unitSetExcelImport;
        public UnitSet unitSet;
        public MissionMaster mission;
        public string targetPathWithoutName;

        //public CalculateUnitStatus calculateUnitStatus;
        //public CalculateUnitStatusMaster calculateUnitStatusMaster;
        private static string _excelPath = "UnitSetExcel";
        private static string _unitSetPath = "UnitSetPath";
        private static string _calculateUnitStatus = "CalculateUnitStatus";


        private int _calclatedUnitStatusId;

        //        private static string _targetPath = "UnitSetObjectPath";





        [MenuItem("Window/Unit Set Excel Converter %#s")]
        static void Init()
        {
            GetWindow(typeof(UnitSetExcelConverter));
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey(_excelPath))
            {
                string excelPath = EditorPrefs.GetString(_excelPath);
                unitSetExcelImport = AssetDatabase.LoadAssetAtPath(excelPath, typeof(UnitSetExcelImport)) as UnitSetExcelImport;
            }

            if (EditorPrefs.HasKey(_unitSetPath))
            {
                targetPathWithoutName = EditorPrefs.GetString(_unitSetPath);
            }

            //if (EditorPrefs.HasKey(_calculateUnitStatus))
            //{
            //    _calclatedUnitStatusId = EditorPrefs.GetInt(_calculateUnitStatus, -1);
            //    calculateUnitStatusMaster = EditorUtility.InstanceIDToObject(_calclatedUnitStatusId) as CalculateUnitStatusMaster;

            //}


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

            GUILayout.Label(targetPathWithoutName ?? " Unselected");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            //calculateUnitStatusMaster = EditorGUILayout.ObjectField("3. Select Calculate unit status game object",
            //                calculateUnitStatusMaster, typeof(CalculateUnitStatusMaster), true) as CalculateUnitStatusMaster;
            //if (calculateUnitStatusMaster)
            //{
            //    if (_calclatedUnitStatusId != calculateUnitStatusMaster.GetInstanceID())
            //    {
            //        _calclatedUnitStatusId = calculateUnitStatusMaster.GetInstanceID();
            //        EditorPrefs.SetInt(_calculateUnitStatus, _calclatedUnitStatusId);
            //        Debug.Log(" Saved id :" + _calclatedUnitStatusId);
            //    }
            //}


            GUILayout.EndHorizontal();


            if (GUILayout.Button("4. Convert Excel to Unit Set", GUILayout.ExpandWidth(false)))
            {
                ConvertUnitSetFromExcel();
            }
            GUILayout.Space(20);
        }

        void OpenUnitMasterExcel()
        {
            string absPath = EditorUtility.OpenFilePanel("Select Unit Set Excel", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitSetExcelImport = AssetDatabase.LoadAssetAtPath(relPath, typeof(UnitSetExcelImport)) as UnitSetExcelImport;
                if (unitSetExcelImport != null && unitSetExcelImport.unitSetExcelList == null)
                    unitSetExcelImport.unitSetExcelList = new List<UnitSetExcel>();
                if (unitSetExcelImport)
                {
                    EditorPrefs.SetString(_excelPath, relPath);
                }
            }
        }

        void OpenTargetList()
        {

            string absPath = EditorUtility.OpenFolderPanel("Select Target List Path", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                targetPathWithoutName = relPath;


                if (targetPathWithoutName != null)
                {
                    EditorPrefs.SetString(_unitSetPath, relPath);
                }
            }
        }


        void ConvertUnitSetFromExcel()
        {
            //SerializedObject missionSerializedObject = new SerializedObject(mission);


            int currentMissionId = 0;
            foreach (var unitMasterExcel in unitSetExcelImport.unitSetExcelList)
            {

                if (unitMasterExcel.missionId != currentMissionId)
                {
                    //new mission start. so creat it.
                    currentMissionId = unitMasterExcel.missionId;


                    // UnitSet Create
                    string _unitSetPath = targetPathWithoutName + "/UnitSet/UnitSet-" + currentMissionId;

                    //load scriptable object from Resources need ref path.
                    var q = "/Resources/";
                    var indexQ = _unitSetPath.IndexOf(q, StringComparison.Ordinal) + q.Length;
                    string _refUnitSetPath = _unitSetPath.Substring(indexQ);

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
                    unitSet.unitSetList = new List<UnitWave>();
                    unitSet.missionId = unitMasterExcel.missionId;

                    //2. Mission Create
                    string _missionPath = targetPathWithoutName + "/Mission-" + currentMissionId;

                    var r = "/Resources/";
                    var indexR = _missionPath.IndexOf(r, StringComparison.Ordinal) + r.Length;
                    string _refMissionPath = _missionPath.Substring(indexR);
                    MissionMaster missionCheck = Resources.Load<MissionMaster>(_refMissionPath);


                    if (missionCheck == null)
                    {
                        mission = MissionCreate.Create(_missionPath + ".asset");
                    }
                    else
                    {
                        mission = missionCheck;
                        //mission.Copy(missionCheck);

                    }


                    mission.category = unitMasterExcel.missionCategory;
                    mission.Id = unitMasterExcel.missionId;
                    mission.missionName = unitMasterExcel.missionString;
                    mission.locationString = unitMasterExcel.locationString;
                    mission.levelInitial = unitMasterExcel.missionLevelInitial;

                    //mission.calculateUnitStatus.master = calculateUnitStatusMaster;


                }

                //var unitWavePath = targetPathWithoutName + "/UnitWave/" + currentMissionId + "-" + unitMasterExcel.waveId + ".asset";
                //Debug.Log("unitMasterExcel: " + unitMasterExcel.missionString);
                string unitWavePath = targetPathWithoutName + "/UnitWave/" + currentMissionId + "-" + unitMasterExcel.waveId + ".asset";
                UnitWave unitWave = unitMasterExcel.GetUnitSet(unitWavePath);
                if (unitWave != null && unitWave.unitWave != null)
                {
                    unitSet.unitSetList.Add(unitWave);
                }


                mission.unitSet = unitSet;
                EditorUtility.SetDirty(unitWave);
                EditorUtility.SetDirty(unitSet);
                EditorUtility.SetDirty(mission);

            }


            //missionSerializedObject.ApplyModifiedProperties();

        }

    }
}
