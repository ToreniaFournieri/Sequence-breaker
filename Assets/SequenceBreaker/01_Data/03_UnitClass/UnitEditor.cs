using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SequenceBreaker._01_Data._03_UnitClass
{
    public class UnitEditor : EditorWindow
    {
        public UnitListScriptable unitListScriptable;
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
                unitListScriptable = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnitListScriptable)) as UnitListScriptable;
            }

        }

        void  OnGUI () {
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("Unit Editor", EditorStyles.boldLabel);
            if (unitListScriptable != null) {
                if (GUILayout.Button("Show Unit List")) 
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = unitListScriptable;
                }
            }
            if (GUILayout.Button("Open Unit List")) 
            {
                OpenItemList();
            }
            if (GUILayout.Button("New Unit List")) 
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = unitListScriptable;
            }
            GUILayout.EndHorizontal ();

            if (unitListScriptable == null) 
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

            if (unitListScriptable != null) 
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
                    if (viewIndex < unitListScriptable.unitList.Count) 
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
                if (unitListScriptable.unitList == null)
                    Debug.Log("wtf");
                if (unitListScriptable.unitList.Count > 0) 
                {
                    GUILayout.BeginHorizontal ();
                    viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Unit", viewIndex, GUILayout.ExpandWidth(false)), 1, unitListScriptable.unitList.Count);
                    //Mathf.Clamp (viewIndex, 1, unitListScriptable.itemList.Count);
                    EditorGUILayout.LabelField ("of   " +  unitListScriptable.unitList.Count.ToString() + "  units", "", GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal ();

                    unitListScriptable.unitList[viewIndex-1].Name = EditorGUILayout.TextField ("Unit Name", unitListScriptable.unitList[viewIndex-1].Name as string);
//                    unitListScriptable.unitListScriptable[viewIndex-1].itemIcon = EditorGUILayout.ObjectField ("Item Icon", unitListScriptable.unitListScriptable[viewIndex-1].itemIcon, typeof (Texture2D), false) as Texture2D;
//                    unitListScriptable.unitListScriptable[viewIndex-1].itemObject = EditorGUILayout.ObjectField ("Item Object", unitListScriptable.unitListScriptable[viewIndex-1].itemObject, typeof (Rigidbody), false) as Rigidbody;

                    GUILayout.Space(10);

//                    GUILayout.BeginHorizontal ();
//                    unitListScriptable.unitListScriptable[viewIndex-1].isUnique = (bool)EditorGUILayout.Toggle("Unique", unitListScriptable.unitListScriptable[viewIndex-1].isUnique, GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].isIndestructible = (bool)EditorGUILayout.Toggle("Indestructable", unitListScriptable.unitListScriptable[viewIndex-1].isIndestructible,  GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].isQuestItem = (bool)EditorGUILayout.Toggle("QuestItem", unitListScriptable.unitListScriptable[viewIndex-1].isQuestItem,  GUILayout.ExpandWidth(false));
//                    GUILayout.EndHorizontal ();

                    GUILayout.Space(10);

//                    GUILayout.BeginHorizontal ();
//                    unitListScriptable.unitListScriptable[viewIndex-1].isStackable = (bool)EditorGUILayout.Toggle("Stackable ", unitListScriptable.unitListScriptable[viewIndex-1].isStackable , GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].destroyOnUse = (bool)EditorGUILayout.Toggle("Destroy On Use", unitListScriptable.unitListScriptable[viewIndex-1].destroyOnUse,  GUILayout.ExpandWidth(false));
//                    unitListScriptable.unitListScriptable[viewIndex-1].encumbranceValue = EditorGUILayout.FloatField("Encumberance", unitListScriptable.unitListScriptable[viewIndex-1].encumbranceValue,  GUILayout.ExpandWidth(false));
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
                EditorUtility.SetDirty(unitListScriptable);
            }
        }

        void CreateNewItemList () 
        {
            // There is no overwrite protection here!
            // There is No "Are you sure you want to overwrite your existing object?" if it exists.
            // This should probably get a string from the user to create a new name and pass it ...
            viewIndex = 1;
            unitListScriptable = CreateUnitList.Create();
            if (unitListScriptable) 
            {
                unitListScriptable.unitList = new List<Unit>();
                string relPath = AssetDatabase.GetAssetPath(unitListScriptable);
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }

        void OpenItemList () 
        {
            string absPath = EditorUtility.OpenFilePanel ("Select Inventory Unit List", "", "");
            if (absPath.StartsWith(Application.dataPath)) 
            {
                string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                unitListScriptable = AssetDatabase.LoadAssetAtPath (relPath, typeof(UnitListScriptable)) as UnitListScriptable;
                if (unitListScriptable.unitList == null)
                    unitListScriptable.unitList = new List<Unit>();
                if (unitListScriptable) {
                    EditorPrefs.SetString("ObjectPath", relPath);
                }
            }
        }

        void AddItem () 
        {
            Unit unit = new Unit();
            unit.Name = "New unit";
            unitListScriptable.unitList.Add (unit);
            viewIndex = unitListScriptable.unitList.Count;
        }

        void DeleteItem (int index) 
        {
            unitListScriptable.unitList.RemoveAt (index);
        }
    }
    
}
