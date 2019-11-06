using System;
using System.Collections.Generic;
using SequenceBreaker._00_System;
using UnityEditor;
using UnityEngine;


namespace SequenceBreaker._01_Data._03_UnitClass.Editor
{
    public class UnitEditor : EditorWindow
    {
        public UnitMasterList unitMasterList;
        private int viewIndex = 1;
        
        [MenuItem ("Window/Unit Master Editor %#e")]
        static void  Init () 
        {
            EditorWindow.GetWindow (typeof (UnitEditor));
        }

        void  OnEnable () {
            if(EditorPrefs.HasKey("ObjectPath")) 
            {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                unitMasterList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnitMasterList)) as UnitMasterList;
            }

        }

        void  OnGUI () {
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("Unit Editor", EditorStyles.boldLabel);
            if (unitMasterList != null) {
                if (GUILayout.Button("Show Unit List")) 
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = unitMasterList;
                }
            }
            if (GUILayout.Button("Open Unit List")) 
            {
                OpenItemList();
            }
            if (GUILayout.Button("New Unit List")) 
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = unitMasterList;
            }
            GUILayout.EndHorizontal ();

            if (unitMasterList == null) 
            {
                GUILayout.BeginHorizontal ();
                GUILayout.Space(10);
                if (GUILayout.Button("Create New Unit List", GUILayout.ExpandWidth(false))) 
                {
                    CreateNewItemList();
                }
                if (GUILayout.Button("Open Existing Unit List", GUILayout.ExpandWidth(false))) 
                {
                    OpenItemList();
                }
                GUILayout.EndHorizontal ();
            }

            GUILayout.Space(20);

            if (unitMasterList != null) 
            {
                
                
                
                GUILayout.BeginHorizontal ();

                GUILayout.Space(10);

                if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false))) 
                {
                    if (viewIndex > 1)
                        viewIndex --;
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Next", GUILayout.ExpandWidth(false))) 
                {
                    if (viewIndex < unitMasterList.unitList.Count) 
                    {
                        viewIndex ++;
                    }
                }

                GUILayout.Space(60);

                if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false))) 
                {
                    AddItem();
                }
                if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false))) 
                {
                    DeleteItem(viewIndex - 1);
                }

                GUILayout.EndHorizontal ();
                if (unitMasterList.unitList == null)
                    Debug.Log("wtf");
                if (unitMasterList.unitList.Count > 0) 
                {
                    GUILayout.BeginHorizontal ();
                    viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Unit", viewIndex, GUILayout.ExpandWidth(false)), 1, unitMasterList.unitList.Count);
                    //Mathf.Clamp (viewIndex, 1, unitListScriptable.itemList.Count);
                    EditorGUILayout.LabelField ("of   " +  unitMasterList.unitList.Count.ToString() + "  units", "", GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal ();
                    
                    // unitList own calculation
                    if (unitMasterList.unitList[viewIndex - 1].autoGenerationMode)
                    {
                        //Get rank
                        char coreFrameC = unitMasterList.unitList[viewIndex - 1].unitName.ToCharArray()[0];
                        char rankC = unitMasterList.unitList[viewIndex - 1].unitName.ToCharArray()[1];
                        char unitTypeC =unitMasterList.unitList[viewIndex - 1].unitName.ToCharArray()[2];

                        //1st coreFrame
                        string coreFramePath = "11_Unit-Base-Master/01_CoreFrame/Core-" + rankC + "-" +coreFrameC;
                        unitMasterList.unitList[viewIndex - 1].coreFrame = Resources.Load<CoreFrame>(coreFramePath);
                        //2nd pilot
                        string pilotPath = "11_Unit-Base-Master/02_Pilot/Pilot-Common" + rankC ;
                        unitMasterList.unitList[viewIndex - 1].pilot = Resources.Load<Pilot.Pilot>(pilotPath);
                        //3rd unitType
                        UnitType unitType = UnitType.Beast;
                        switch (unitTypeC)
                        {
                            case 'B':
                                unitType = UnitType.Beast;
                                break;
                            case 'C':
                                unitType = UnitType.Cyborg;
                                break;
                            case 'D':
                                    unitType = UnitType.Drone;
                                    break;
                            case 'R':
                                unitType = UnitType.Robot;
                                break;
                            case 'T':
                                unitType = UnitType.Titan;
                                break;
                            default:
//                                Debug.LogError("unexpected unitType :" + unitTypeC);
                                break;
                        }

                        unitMasterList.unitList[viewIndex - 1].unitType = unitType;


                    }
                        

                    unitMasterList.unitList[viewIndex-1].unitName = EditorGUILayout.TextField ("Unit Name", unitMasterList.unitList[viewIndex-1].unitName as string);
                    unitMasterList.unitList[viewIndex-1].autoGenerationMode = EditorGUILayout.Toggle("autoGenerationMode", unitMasterList.unitList[viewIndex-1].autoGenerationMode );

                    GUILayout.Label(" true:will generate unit Type from Unit name, 1st:CoreFrame, 2nd:Rank, 3rd:UnitType" );
                    unitMasterList.unitList[viewIndex - 1].uniqueId =
                        Math.Abs(unitMasterList.unitList[viewIndex - 1].unitName.GetHashCode());
                    unitMasterList.unitList[viewIndex-1].uniqueId = EditorGUILayout.IntField("UniqueId", unitMasterList.unitList[viewIndex-1].uniqueId );
                    unitMasterList.unitList[viewIndex - 1].affiliation = (Affiliation) EditorGUILayout.EnumPopup("Affiliation",
                        unitMasterList.unitList[viewIndex - 1].affiliation );
                    unitMasterList.unitList[viewIndex - 1].unitType = (UnitType) EditorGUILayout.EnumPopup("UnitType",
                        unitMasterList.unitList[viewIndex - 1].unitType );
                    unitMasterList.unitList[viewIndex - 1].itemCapacity = EditorGUILayout.IntField("itemCapacity", unitMasterList.unitList[viewIndex - 1].itemCapacity );
                    

                    GUILayout.Space(10);

                    unitMasterList.unitList[viewIndex - 1].coreFrame =
                            EditorGUILayout.ObjectField("CoreFrame", unitMasterList.unitList[viewIndex - 1].coreFrame, typeof (CoreFrame), false) as CoreFrame; 
                    unitMasterList.unitList[viewIndex - 1].pilot =
                        EditorGUILayout.ObjectField("Pilot", unitMasterList.unitList[viewIndex - 1].pilot, typeof (Pilot.Pilot), false) as Pilot.Pilot; 
                    
                    unitMasterList.unitList[viewIndex-1].experience = EditorGUILayout.IntField("experience", unitMasterList.unitList[viewIndex-1].experience );



//                    unitListScriptable.unitListScriptable[viewIndex-1].itemIcon = EditorGUILayout.ObjectField ("Item Icon", unitListScriptable.unitListScriptable[viewIndex-1].itemIcon, typeof (Texture2D), false) as Texture2D;
//                    unitListScriptable.unitListScriptable[viewIndex-1].itemObject = EditorGUILayout.ObjectField ("Item Object", unitListScriptable.unitListScriptable[viewIndex-1].itemObject, typeof (Rigidbody), false) as Rigidbody;
//                    unitListScriptable.unitListScriptable[viewIndex-1].isUnique = (bool)EditorGUILayout.Toggle("Unique", unitListScriptable.unitListScriptable[viewIndex-1].isUnique, GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].isIndestructible = (bool)EditorGUILayout.Toggle("Indestructable", unitListScriptable.unitListScriptable[viewIndex-1].isIndestructible,  GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].isQuestItem = (bool)EditorGUILayout.Toggle("QuestItem", unitListScriptable.unitListScriptable[viewIndex-1].isQuestItem,  GUILayout.ExpandWidth(false));

//                    GUILayout.BeginHorizontal ();
//                    unitListScriptable.unitListScriptable[viewIndex-1].isStackable = (bool)EditorGUILayout.Toggle("Stackable ", unitListScriptable.unitListScriptable[viewIndex-1].isStackable , GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].destroyOnUse = (bool)EditorGUILayout.Toggle("Destroy On Use", unitListScriptable.unitListScriptable[viewIndex-1].destroyOnUse,  GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].encumbranceValue = EditorGUILayout.FloatField("Encumberance", unitListScriptable.unitListScriptable[viewIndex-1].encumbranceValue,  GUILayout.ExpandWidth(false));
//                    GUILayout.EndHorizontal ();

                } 
                else 
                {
                    GUILayout.Label ("This Unit List is Empty.");
                }
            }
            if (GUI.changed) 
            {
                EditorUtility.SetDirty(unitMasterList);
            }
        }

        void CreateNewItemList () 
        {
            // There is no overwrite protection here!
            // There is No "Are you sure you want to overwrite your existing object?" if it exists.
            // This should probably get a string from the user to create a new unitName and pass it ...
            viewIndex = 1;
            unitMasterList = CreateUnitList.Create();
            if (unitMasterList) 
            {
                unitMasterList.unitList = new List<UnitMaster>();
                string relPath = AssetDatabase.GetAssetPath(unitMasterList);
                EditorPrefs.SetString("ObjectPath", relPath);
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
                    EditorPrefs.SetString("ObjectPath", relPath);
                }
            }
        }

        void AddItem () 
        {
            UnitMaster unitMaster = new UnitMaster();
            unitMaster.unitName = "DAR-Destroyer";
            unitMaster.affiliation = Affiliation.Enemy;
            unitMaster.itemCapacity = 1;
//            unitMaster.coreFrame = Resources.Load<CoreFrame>
//                ("11_Unit-Base-Master/01_CoreFrame/Core-A-Destroyer");
//            unitMaster.pilot = Resources.Load<Pilot.Pilot>("11_Unit-Base-Master/02_Pilot/Pilot-CommonA");
            unitMaster.autoGenerationMode = true;
            unitMasterList.unitList.Add (unitMaster);
            viewIndex = unitMasterList.unitList.Count;
        }

        void DeleteItem (int index) 
        {
            unitMasterList.unitList.RemoveAt (index);
        }
    }
    
}
