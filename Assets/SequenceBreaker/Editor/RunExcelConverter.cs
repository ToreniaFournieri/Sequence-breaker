using System;
using System.Collections.Generic;
using SequenceBreaker.Editor;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    public class RunExcelConverter : EditorWindow
    {
        public UnitMasterExcelImport unitMasterExcelImport;
        //        public UnitMasterList unitMasterList;
        public UnitClassList unitClassList;

        public string unitClassListPath;

        public string unitPath;
        public string itemPresetPath;

        private UnitClass _unit;

        private int viewIndex = 1;

        private static string _excelPathId = "ExcelPath";
        //        private static string _targetPathId = "ObjectPath";

        private static string _unitClassListPathId = "unitClassListPath";
        private static string _unitClassPathId = "unitClassPath";
        private static string _itemPresetPathId = "itemPresetPath";

        // Unit Set
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


        [MenuItem("Window/Unit Master Excel Converter %#e")]
        static void Init()
        {
            GetWindow(typeof(RunExcelConverter));
        }

        private void OnEnable()
        {
            //1. Select [Import] Unit Master Excel asset
            if (EditorPrefs.HasKey(_excelPathId))
            {
                string excelPath = EditorPrefs.GetString(_excelPathId);
                unitMasterExcelImport = AssetDatabase.LoadAssetAtPath(excelPath, typeof(UnitMasterExcelImport)) as UnitMasterExcelImport;
            }

            //2. Select [Export] Unit Class List asset
            if (EditorPrefs.HasKey(_unitClassListPathId))
            {
                string objectPath = EditorPrefs.GetString(_unitClassListPathId);
                unitClassList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnitClassList)) as UnitClassList;
            }


            //3. Select [Export] Unit Class Path
            if (EditorPrefs.HasKey(_unitClassPathId))
            {
                string objectPath = EditorPrefs.GetString(_unitClassPathId);
                //unitPath = AssetDatabase.LoadAssetAtPath(objectPath, typeof(string)).ToString();
                unitPath = objectPath;

                //Debug.Log("unitPath: " + unitPath);
            }

            //4. Select [Export] Item Preset Path
            if (EditorPrefs.HasKey(_itemPresetPathId))
            {
                string objectPath = EditorPrefs.GetString(_itemPresetPathId);
                itemPresetPath = objectPath;

            }

            if (EditorPrefs.HasKey(_excelPath))
            {
                string excelPath = EditorPrefs.GetString(_excelPath);
                unitSetExcelImport = AssetDatabase.LoadAssetAtPath(excelPath, typeof(UnitSetExcelImport)) as UnitSetExcelImport;
            }

            if (EditorPrefs.HasKey(_unitSetPath))
            {
                targetPathWithoutName = EditorPrefs.GetString(_unitSetPath);
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

            if (GUILayout.Button("2. Select [Export] Unit Class List asset", GUILayout.ExpandWidth(false)))
            {
                OpenUnitClassList();
            }

            GUILayout.Label(unitClassList ? unitClassList.name : " Unselected");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("3. Select [Export] Unit Class Path", GUILayout.ExpandWidth(false)))
            {
                OpenUnitClass();
            }

            GUILayout.Label(unitPath ?? " Unselected");

            GUILayout.EndHorizontal();
            GUILayout.Label("/07_ScriptableObject/Resources/20_Enemy/UnitList is correct path");


            GUILayout.BeginHorizontal();


            if (GUILayout.Button("4. Select [Export] Item Preset path", GUILayout.ExpandWidth(false)))
            {
                OpenItemPreset();
            }
            GUILayout.Label(itemPresetPath ?? " Unselected");

            GUILayout.EndHorizontal();
            GUILayout.Label("/07_ScriptableObject/Resources/22_EnemyItemPreset is correct path");


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("5. Select [Import] Unit Set Excel asset", GUILayout.ExpandWidth(false)))
            {
                OpenUnitSetMasterExcel();
            }

            GUILayout.Label(unitSetExcelImport ? unitSetExcelImport.name : " Unselected");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("6. Select [Export] Unit Set path", GUILayout.ExpandWidth(false)))
            {
                OpenTargetList();
            }

            GUILayout.Label(targetPathWithoutName ?? " Unselected");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();


            GUILayout.EndHorizontal();


            if (GUILayout.Button("10.[PRESS] Convert Excel to ScriptableObject", GUILayout.ExpandWidth(false)))
            {
                ConvertUnitListFromExcel();
                ConvertUnitSetFromExcel();

            }
            GUILayout.Space(20);


        }

        void OpenUnitMasterExcel()
        {
            string absPath = EditorUtility.OpenFilePanel("Select Unit Master Excel", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);

                unitMasterExcelImport = AssetDatabase.LoadAssetAtPath(relPath, typeof(UnitMasterExcelImport)) as UnitMasterExcelImport;
                if (unitMasterExcelImport.unitMasterExcel == null)
                    unitMasterExcelImport.unitMasterExcel = new List<UnitMasterExcel>();
                if (unitMasterExcelImport)
                {
                    EditorPrefs.SetString(_excelPathId, relPath);
                }
            }
        }

        void OpenUnitClassList()
        {
            string absPath = EditorUtility.OpenFilePanel("Select Unit Class List", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitClassListPath = relPath;
                unitClassList = AssetDatabase.LoadAssetAtPath(relPath, typeof(UnitClassList)) as UnitClassList;
                if (unitClassList != null && unitClassList.unitList == null)
                    unitClassList.unitList = new List<UnitClass>();
                if (unitClassList)
                {
                    EditorPrefs.SetString(_unitClassListPathId, relPath);
                }
            }
        }


        void OpenUnitClass()
        {
            string absPath = EditorUtility.OpenFolderPanel("Select Unit Class path", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitPath = relPath;

                if (unitPath != null)
                {
                    EditorPrefs.SetString(_unitClassPathId, unitPath);
                }
            }

        }

        void OpenItemPreset()
        {
            string absPath = EditorUtility.OpenFolderPanel("Select Item Preset path", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                itemPresetPath = relPath;

                if (unitPath != null)
                {
                    EditorPrefs.SetString(_unitClassPathId, unitPath);
                }
            }

        }


        void ConvertUnitListFromExcel()
        {
            // There is no overwrite protection here!
            // There is No "Are you sure you want to overwrite your existing object?" if it exists.
            // This should probably get a string from the user to create a new unitName and pass it ...
            viewIndex = 1;


            if (unitClassList == null)
            {
                Debug.Log("changed");
                unitClassList = UnitClassListCreate.Create(unitClassListPath);
            }
            unitClassList.unitList = new List<UnitClass>();

            string itemListPath = itemPresetPath + "/itemPresetList" + ".asset";

            ItemPresetList itemPresetList = ItemPresetListCreate.Create(itemListPath);
            itemPresetList.itemPresetList = new List<ItemPreset>();

            foreach (UnitMasterExcel unitMasterExcel in unitMasterExcelImport.unitMasterExcel)
            {
                _unit = null;
                _unit = UnitCreate.Create(unitPath, unitMasterExcel.GetUnitClass());
                unitClassList.unitList.Add(_unit);
                _unit.itemList = new List<Item>();


                string itemPath = itemPresetPath + "/itemPreset-" + _unit.uniqueId + ".asset" ;

                

                ItemPreset itemPreset = ItemPresetCreate.Create(itemPath);



                foreach (var unitEquip in unitMasterExcelImport.unitEquipment)
                {
                    if (_unit.uniqueId == unitEquip.uniqueId)
                    {

                        itemPreset.characterUniqueId = _unit.uniqueId;
                        itemPreset.itemIdList = new List<ItemIdSet>();
                        ItemIdSet itemIdSet = new ItemIdSet();

                        if (unitEquip.equipment1 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment1, 0, 1)); }
                        if (unitEquip.equipment2 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment2, 0, 1)); }
                        if (unitEquip.equipment3 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment3, 0, 1)); }
                        if (unitEquip.equipment4 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment4, 0, 1)); }
                        if (unitEquip.equipment5 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment5, 0, 1)); }
                        if (unitEquip.equipment6 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment6, 0, 1)); }
                        if (unitEquip.equipment7 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment7, 0, 1)); }
                        if (unitEquip.equipment8 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment8, 0, 1)); }
                        if (unitEquip.equipment9 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment9, 0, 1)); }
                        if (unitEquip.equipment10 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment10, 0, 1)); }
                        if (unitEquip.equipment11 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment11, 0, 1)); }
                        if (unitEquip.equipment12 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment12, 0, 1)); }
                        if (unitEquip.equipment13 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment13, 0, 1)); }
                        if (unitEquip.equipment14 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment14, 0, 1)); }
                        if (unitEquip.equipment15 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment15, 0, 1)); }
                        if (unitEquip.equipment16 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment16, 0, 1)); }
                        if (unitEquip.equipment17 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment17, 0, 1)); }
                        if (unitEquip.equipment18 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment18, 0, 1)); }
                        if (unitEquip.equipment19 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment19, 0, 1)); }
                        if (unitEquip.equipment20 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment20, 0, 1)); }
                        if (unitEquip.equipment21 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment21, 0, 1)); }
                        if (unitEquip.equipment22 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment22, 0, 1)); }
                        if (unitEquip.equipment23 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment23, 0, 1)); }
                        if (unitEquip.equipment24 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment24, 0, 1)); }
                        if (unitEquip.equipment25 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment25, 0, 1)); }
                        if (unitEquip.equipment26 != 0) { itemPreset.itemIdList.Add(itemIdSet.Add(0, unitEquip.equipment26, 0, 1)); }
                    }
                }
                EditorUtility.SetDirty(itemPreset);
                itemPresetList.itemPresetList.Add(itemPreset);

                EditorUtility.SetDirty(itemPresetList);

            }
            EditorUtility.SetDirty(unitClassList);


        }

        void OpenUnitSetMasterExcel()
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
