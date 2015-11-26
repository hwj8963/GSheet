using UnityEngine;
using UnityEditor;
using System.IO;

public class GSheetSettings : ScriptableObject {



	public string CLIENT_ID;
	public string CLIENT_SECRET;


	static readonly string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";
	static readonly string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";


















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
