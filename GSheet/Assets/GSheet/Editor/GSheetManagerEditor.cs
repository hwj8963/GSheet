using UnityEngine;
using UnityEditor;
using System.Collections;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System.Text;

[CustomEditor(typeof(GSheetManager))]
public class GSheetManagerEditor : Editor {

	GSheetManager manager;

	string SpreadSheetName = "";
	string WorkSheetName = "";
	bool CreateDataAsset = false;
	void OnEnable() {
		manager = target as GSheetManager;
	}

	public override void OnInspectorGUI ()
	{
		SpreadSheetName = EditorGUILayout.TextField("SpreadSheet Name",SpreadSheetName);
		WorkSheetName = EditorGUILayout.TextField("WorkSheet Name",WorkSheetName);
		CreateDataAsset = EditorGUILayout.Toggle ("Create Data Asset", CreateDataAsset);
		if (GUILayout.Button ("Create Data Script")) {
			CreateScript();
		}

	}
	public void CreateScript() {
		GSheetSettings setting = GSheetUtility.GetSettings ();
		if (setting == null)
			return;
		SpreadsheetsService service = setting.GetService ();
		
		WorksheetEntry worksheet = setting.GetWorkSheet (service, SpreadSheetName, WorkSheetName);


		CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
		CellFeed cellFeed = service.Query(cellQuery);

		string fieldFormat = setting.FieldTemplate.text;
		StringBuilder sb = new StringBuilder ();
		// Iterate through each cell, printing its value.
		foreach (CellEntry cell in cellFeed.Entries)
		{
			if(cell.Row > 1 ) 
			{
				break;
			}
			string fieldType = "string";
			if(cell.Value[0] == 'n') {
				fieldType = "int";
			} else if(cell.Value[1] == 'f') {
				fieldType = "float";
         	}
			string fieldScript = fieldFormat.Replace("{FieldName}",cell.Value).Replace("{LowerCaseFieldName}",cell.Value.ToLower()).Replace ("{FieldType}",fieldType);
			sb.Append (fieldScript);
		}

		string dataFormat = setting.DataTemplate.text;
		string dataScript = dataFormat.Replace ("{WorkSheetName}", WorkSheetName).Replace ("{FieldList}", sb.ToString ());
		StringBuilder dataPathSB = new StringBuilder(setting.ScriptPath);

		if (setting.ScriptPath[setting.ScriptPath.Length - 1] != '/') {
			dataPathSB.Append("/");
		}
		dataPathSB.Append (WorkSheetName);
		dataPathSB.Append ("Data.cs");

		System.IO.File.WriteAllText (dataPathSB.ToString (), dataScript);


		string editorFormat = setting.DataEditorTemplate.text;
		string editorScript = editorFormat.Replace ("{WorkSheetName}", WorkSheetName);
		StringBuilder editorPathSB = new StringBuilder (setting.EditorScriptPath);

		if (setting.EditorScriptPath [setting.EditorScriptPath.Length - 1] != '/') {
			editorPathSB.Append("/");
		}
		editorPathSB.Append (WorkSheetName);
		editorPathSB.Append ("DataEditor.cs");

		System.IO.File.WriteAllText (editorPathSB.ToString (), editorScript);

	}


	[MenuItem("Assets/Create/Custom/GSheet/Create GSheet Manager")]
	public static void CreateSettings() {
		var setting = AssetDatabase.FindAssets ("t:GSheetManager");
		if (setting.Length > 0) {
			Debug.Log ("manager already exist : " + AssetDatabase.GUIDToAssetPath(setting[0]));
		} else {
			GSheetUtility.CreateAsset<GSheetManager>();
		}
	}
}
