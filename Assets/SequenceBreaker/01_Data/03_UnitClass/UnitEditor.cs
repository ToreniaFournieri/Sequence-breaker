using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SequenceBreaker._01_Data._03_UnitClass
{
    public class UnitEditor : EditorWindow
    {
        public UnitClassList unitClassList;
        private int viewIndex = 1;
        
        [MenuItem ("Window/Unit List Editor %#e")]
        static void  Init () 
        {
            EditorWindow.GetWindow (typeof (UnitEditor));
        }

        void  OnEnable () {
            if(EditorPrefs.HasKey("ObjectPath")) 
            {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                unitClassList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnitClassList)) as UnitClassList;
            }

        }

        void  OnGUI () {
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("Unit Editor", EditorStyles.boldLabel);
            if (unitClassList != null) {
                if (GUILayout.Button("Show Unit List")) 
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = unitClassList;
                }
            }
            if (GUILayout.Button("Open Unit List")) 
            {
                OpenItemList();
            }
            if (GUILayout.Button("New Unit List")) 
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = unitClassList;
            }
            GUILayout.EndHorizontal ();

            if (unitClassList == null) 
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

            if (unitClassList != null) 
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
                    if (viewIndex < unitClassList.unitList.Count) 
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
                if (unitClassList.unitList == null)
                    Debug.Log("wtf");
                if (unitClassList.unitList.Count > 0) 
                {
                    GUILayout.BeginHorizontal ();
                    viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Unit", viewIndex, GUILayout.ExpandWidth(false)), 1, unitClassList.unitList.Count);
                    //Mathf.Clamp (viewIndex, 1, unitClassList.itemList.Count);
                    EditorGUILayout.LabelField ("of   " +  unitClassList.unitList.Count.ToString() + "  units", "", GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal ();

                    unitClassList.unitList[viewIndex-1].Name = EditorGUILayout.TextField ("Unit Name", unitClassList.unitList[viewIndex-1].Name as string);
//                    unitClassList.unitList[viewIndex-1].itemIcon = EditorGUILayout.ObjectField ("Item Icon", unitClassList.unitList[viewIndex-1].itemIcon, typeof (Texture2D), false) as Texture2D;
//                    unitClassList.unitList[viewIndex-1].itemObject = EditorGUILayout.ObjectField ("Item Object", unitClassList.unitList[viewIndex-1].itemObject, typeof (Rigidbody), false) as Rigidbody;

                    GUILayout.Space(10);

//                    GUILayout.BeginHorizontal ();
//                    unitClassList.unitList[viewIndex-1].isUnique = (bool)EditorGUILayout.Toggle("Unique", unitClassList.unitList[viewIndex-1].isUnique, GUILayout.ExpandWidth(false));
//                    unitClassList.unitList[viewIndex-1].isIndestructible = (bool)EditorGUILayout.Toggle("Indestructable", unitClassList.unitList[viewIndex-1].isIndestructible,  GUILayout.ExpandWidth(false));
//                    unitClassList.unitList[viewIndex-1].isQuestItem = (bool)EditorGUILayout.Toggle("QuestItem", unitClassList.unitList[viewIndex-1].isQuestItem,  GUILayout.ExpandWidth(false));
//                    GUILayout.EndHorizontal ();

                    GUILayout.Space(10);

//                    GUILayout.BeginHorizontal ();
//                    unitClassList.unitList[viewIndex-1].isStackable = (bool)EditorGUILayout.Toggle("Stackable ", unitClassList.unitList[viewIndex-1].isStackable , GUILayout.ExpandWidth(false));
//                    unitClassList.unitList[viewIndex-1].destroyOnUse = (bool)EditorGUILayout.Toggle("Destroy On Use", unitClassList.unitList[viewIndex-1].destroyOnUse,  GUILayout.ExpandWidth(false));
//                    unitClassList.unitList[viewIndex-1].encumbranceValue = EditorGUILayout.FloatField("Encumberance", unitClassList.unitList[viewIndex-1].encumbranceValue,  GUILayout.ExpandWidth(false));
//                    GUILayout.EndHorizontal ();

                    GUILayout.Space(10);

                } 
                else 
                {
                    GUILayout.Label ("This Unit List is Empty.");
                }
            }
            if (GUI.changed) 
            {
                EditorUtility.SetDirty(unitClassList);
            }
        }

        void CreateNewItemList () 
        {
            // There is no overwrite protection here!
            // There is No "Are you sure you want to overwrite your existing object?" if it exists.
            // This should probably get a string from the user to create a new name and pass it ...
            viewIndex = 1;
            unitClassList = CreateUnitList.Create();
            if (unitClassList) 
            {
                unitClassList.unitList = new List<Unit>();
                string relPath = AssetDatabase.GetAssetPath(unitClassList);
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }

        void OpenItemList () 
        {
            string absPath = EditorUtility.OpenFilePanel ("Select Inventory Unit List", "", "");
            if (absPath.StartsWith(Application.dataPath)) 
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitClassList = AssetDatabase.LoadAssetAtPath (relPath, typeof(UnitClassList)) as UnitClassList;
                if (unitClassList.unitList == null)
                    unitClassList.unitList = new List<Unit>();
                if (unitClassList) {
                    EditorPrefs.SetString("ObjectPath", relPath);
                }
            }
        }

        void AddItem () 
        {
            Unit unit = new Unit();
            unit.Name = "New unit";
            unitClassList.unitList.Add (unit);
            viewIndex = unitClassList.unitList.Count;
        }

        void DeleteItem (int index) 
        {
            unitClassList.unitList.RemoveAt (index);
        }
    }
    
}
