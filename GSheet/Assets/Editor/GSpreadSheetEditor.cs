﻿using UnityEngine;
using UnityEditor;
using System.IO;
[CustomEditor(typeof(GSpreadSheet))]
public class GSpreadSheetEditor : Editor {
	GSpreadSheet spreadSheet;
	void OnEnable() {
		spreadSheet = target as GSpreadSheet;
	}
	public override void OnInspectorGUI ()
	{
		spreadSheet.SpreadSheetName = EditorGUILayout.TextField ("SpreadSheet Name", spreadSheet.SpreadSheetName);
		if (GUILayout.Button ("Find SpreadSheet")) {
			spreadSheet.FindSpreadSheet();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(spreadSheet);
		}
	}

	[MenuItem("Custom/GSheet/Create GSpreadSheet")]
	public static void CreateSpreadSheet() {
		GSpreadSheet asset = ScriptableObject.CreateInstance<GSpreadSheet> ();
			
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New GSpreadSheet.asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

}