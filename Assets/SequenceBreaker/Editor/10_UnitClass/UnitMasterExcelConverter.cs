using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.UnitClass;
using UnityEditor;
using UnityEngine;

namespace SequenceBreaker.Editor._10_UnitClass
{
    public class UnitMasterExcelConverter : EditorWindow
    {
        public UnitMasterExcelImport unitMasterExcelImport;
        //        public UnitMasterList unitMasterList;
        public UnitClassList unitClassList;

        public string unitClassListPath;

        public string unitPath;

        private UnitClass _unit;

        private int viewIndex = 1;

        private static string _excelPathId = "ExcelPath";
        //        private static string _targetPathId = "ObjectPath";

        private static string _unitClassListPathId = "unitClassListPath";
        private static string _unitClassPathId = "unitClassPath";


        [MenuItem("Window/Unit Master Excel Converter %#e")]
        static void Init()
        {
            GetWindow(typeof(UnitMasterExcelConverter));
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



            if (GUILayout.Button("4. Convert Excel to Unit List", GUILayout.ExpandWidth(false)))
            {
                ConvertUnitListFromExcel();
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

            foreach (var unitEquip in unitMasterExcelImport.unitEquipment)
            {

            }



            foreach (UnitMasterExcel unitMasterExcel in unitMasterExcelImport.unitMasterExcel)
            {
                _unit = null;
                _unit = UnitCreate.Create(unitPath, unitMasterExcel.GetUnitClass());
                unitClassList.unitList.Add(_unit);
                _unit.itemList = new List<Item>();

                foreach (var unitEquip in unitMasterExcelImport.unitEquipment)
                {
                    if (_unit.uniqueId == unitEquip.uniqueId)
                    {
                        //Debug.Log(" unit: " + _unit.TrueName() + " equipment id: " + unitEquip.equipment1);
                        //Debug.Log("itemDatabese:" + ItemDataBase.Get.itemBaseMasterList.Count);
                        if (unitEquip.equipment1 != 0)
                        {
                            Item newItem = ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment1, 0, 1);

                            _unit.itemList.Add(newItem);
                        }
                        if (unitEquip.equipment2 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment2, 0, 1)); }
                        if (unitEquip.equipment3 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment3, 0, 1)); }
                        if (unitEquip.equipment4 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment4, 0, 1)); }
                        if (unitEquip.equipment5 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment5, 0, 1)); }
                        if (unitEquip.equipment6 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment6, 0, 1)); }
                        if (unitEquip.equipment7 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment7, 0, 1)); }
                        if (unitEquip.equipment8 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment8, 0, 1)); }
                        if (unitEquip.equipment9 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment9, 0, 1)); }
                        if (unitEquip.equipment10 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment10, 0, 1)); }
                        if (unitEquip.equipment11 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment11, 0, 1)); }
                        if (unitEquip.equipment12 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment12, 0, 1)); }
                        if (unitEquip.equipment13 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment13, 0, 1)); }
                        if (unitEquip.equipment14 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment14, 0, 1)); }
                        if (unitEquip.equipment15 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment15, 0, 1)); }
                        if (unitEquip.equipment16 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment16, 0, 1)); }
                        if (unitEquip.equipment17 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment17, 0, 1)); }
                        if (unitEquip.equipment18 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment18, 0, 1)); }
                        if (unitEquip.equipment19 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment19, 0, 1)); }
                        if (unitEquip.equipment20 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment20, 0, 1)); }
                        if (unitEquip.equipment21 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment21, 0, 1)); }
                        if (unitEquip.equipment22 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment22, 0, 1)); }
                        if (unitEquip.equipment23 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment23, 0, 1)); }
                        if (unitEquip.equipment24 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment24, 0, 1)); }
                        if (unitEquip.equipment25 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment25, 0, 1)); }
                        if (unitEquip.equipment26 != 0) { _unit.itemList.Add(ItemDataBase.Get.GetItemFromId(0, unitEquip.equipment26, 0, 1)); }
                    }
                }


            }
            EditorUtility.SetDirty(unitClassList);


        }
    }
}
