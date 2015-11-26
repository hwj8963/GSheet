using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GSheetSettings))]
public class GSheetsettingsEditor : Editor {
	GSheetSettings settings;
	void OnEnable() {
		settings = target as GSheetSettings;
	}
	public override void OnInspectorGUI ()
	{
		settings.CLIENT_ID = EditorGUILayout.TextField ("Client ID", settings.CLIENT_ID);
		settings.CLIENT_SECRET = EditorGUILayout.TextField ("Client Secret", settings.CLIENT_SECRET);

		if (GUILayout.Button ("Get Access Code")) {
			settings.GetAccessCode();
		}

		settings.ACCESS_CODE = EditorGUILayout.TextField ("Access Code", settings.ACCESS_CODE);

		if (GUILayout.Button ("Get Access Token")) {
			settings.GetAccessToken();
		}
		//settings.ACCESS_TOKEN = EditorGUILayout.TextField ("Access Token", settings.ACCESS_TOKEN);
		EditorGUILayout.LabelField ("Access Token", settings.ACCESS_TOKEN,GUILayout.Height(100f));
		if (GUI.changed) {
			EditorUtility.SetDirty(settings);
		}
	}

	[MenuItem("Custom/GSheet/Create GSheet Settings")]
	public static void CreateSettings() {
		var setting = AssetDatabase.FindAssets ("t:GSheetSettings");
		if (setting.Length > 0) {
			Debug.Log ("setting already exist : " + AssetDatabase.GUIDToAssetPath(setting[0]));
		} else {
			GSheetSettings asset = ScriptableObject.CreateInstance<GSheetSettings> ();
			
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "") 
			{
				path = "Assets";
			} 
			else if (Path.GetExtension (path) != "") 
			{
				path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
			
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/CreateGSheetSettings.asset");
			
			AssetDatabase.CreateAsset (asset, assetPathAndName);
			
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
		}
		
		
	}
}
