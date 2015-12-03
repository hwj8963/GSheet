using UnityEngine;
using UnityEditor;
using System.Collections;
using Google.GData.Spreadsheets;
using Google.GData.Client;

using System;
using System.Reflection;

[CustomEditor(typeof(Sheet1Data))]
public class Sheet1DataEditor : Editor {

	Sheet1Data sheet;
	void OnEnable() {
		sheet = target as Sheet1Data;
	}

	static readonly float buttonWidth = 25f;
	static readonly float fieldMinWidth = 20f;
	public override void OnInspectorGUI ()
	{
		if (GUILayout.Button ("Download From Google Spread Sheet")) {
			Download();
			EditorUtility.SetDirty(sheet);
		}

		sheet.SpreadSheetName = EditorGUILayout.TextField ("Spread Sheet Name", sheet.SpreadSheetName);
		sheet.WorkSheetName = EditorGUILayout.TextField ("Work Sheet Name", sheet.WorkSheetName);

		GUILayout.BeginHorizontal ();

		GUILayout.Label ("sName".PadRight(10,' '),GUILayout.MinWidth (fieldMinWidth));
		GUILayout.Label ("nAge".PadRight(10,' '),GUILayout.MinWidth (fieldMinWidth));
		GUILayout.Label ("nGender".PadRight(10,' '),GUILayout.MinWidth (fieldMinWidth));
		GUILayout.Label ("nScore".PadRight(10,' '),GUILayout.MinWidth (fieldMinWidth));
		GUILayout.Space (4 * buttonWidth + 16);
		GUILayout.EndHorizontal ();
		for (int i=0; i<sheet.data.Count; i++) {
			GUILayout.BeginHorizontal();
			sheet.data[i].sName = EditorGUILayout.TextField(sheet.data[i].sName,GUILayout.MinWidth(fieldMinWidth));
			sheet.data[i].nAge = EditorGUILayout.IntField(sheet.data[i].nAge,GUILayout.MinWidth(fieldMinWidth));
			sheet.data[i].nGender = EditorGUILayout.IntField(sheet.data[i].nGender,GUILayout.MinWidth(fieldMinWidth));
			sheet.data[i].nScore = EditorGUILayout.IntField(sheet.data[i].nScore,GUILayout.MinWidth(fieldMinWidth));

			if(GUILayout.Button ("+",GUILayout.Width(buttonWidth))) {
				sheet.data.Insert(i,new Sheet1DataEntry());
			} 
			if(GUILayout.Button ("-",GUILayout.Width(buttonWidth))) {
				sheet.data.RemoveAt(i);
			} 
			if(GUILayout.Button ("▲",GUILayout.Width (buttonWidth))) {
				if(i > 0) {
					Sheet1DataEntry entry = sheet.data[i];
					sheet.data[i] = sheet.data[i-1];
					sheet.data[i-1] = entry;
				}
			} 
			if(GUILayout.Button ("▼",GUILayout.Width (buttonWidth))) {
				if(i < sheet.data.Count-1) {
					Sheet1DataEntry entry = sheet.data[i];
					sheet.data[i] = sheet.data[i+1];
					sheet.data[i+1] = entry;
				}
			}

			GUILayout.EndHorizontal();
		}


		/*
		GUILayout.BeginHorizontal ();

		GUILayout.BeginVertical (GUILayout.MinWidth(fieldMinWidth),GUILayout.ExpandWidth(true)); 
		{
			GUILayout.Label ("sName");
			for (int i=0; i<sheet.data.Count; i++) {
				sheet.data [i].sName = EditorGUILayout.TextField (sheet.data [i].sName);
			}
		}
		GUILayout.EndVertical();

		GUILayout.BeginVertical (GUILayout.MinWidth(fieldMinWidth),GUILayout.ExpandWidth(true)); 
		{
			GUILayout.Label ("nAge");
			for (int i=0; i<sheet.data.Count; i++) {
				sheet.data [i].nAge = EditorGUILayout.IntField (sheet.data [i].nAge);
			}
		}
		GUILayout.EndVertical();

		GUILayout.BeginVertical (GUILayout.MinWidth(fieldMinWidth),GUILayout.ExpandWidth(true)); 
		{
			GUILayout.Label ("nGender");
			for (int i=0; i<sheet.data.Count; i++) {
				sheet.data [i].nGender = EditorGUILayout.IntField (sheet.data [i].nGender);
			}
		}
		GUILayout.EndVertical();

		GUILayout.BeginVertical (GUILayout.MinWidth(fieldMinWidth),GUILayout.ExpandWidth(true)); 
		{
			GUILayout.Label ("nScore");
			for (int i=0; i<sheet.data.Count; i++) {
				sheet.data [i].nScore = EditorGUILayout.IntField (sheet.data [i].nScore);
			}
		}
		GUILayout.EndVertical();

		GUILayout.BeginVertical (GUILayout.MinWidth(fieldMinWidth),GUILayout.ExpandWidth(true)); 
		{
			GUILayout.Label ("");
			for (int i=0; i<sheet.data.Count; i++) {
				GUILayout.BeginHorizontal();
				if(GUILayout.Button ("+",GUILayout.Width(buttonWidth))) {
					sheet.data.Insert(i,new Sheet1DataEntry());
				} 
				if(GUILayout.Button ("-",GUILayout.Width(buttonWidth))) {
					sheet.data.RemoveAt(i);
				} 
				if(GUILayout.Button ("▲",GUILayout.Width (buttonWidth))) {
					if(i > 0) {
						Sheet1DataEntry entry = sheet.data[i];
						sheet.data[i] = sheet.data[i-1];
						sheet.data[i-1] = entry;
					}
				} 
				if(GUILayout.Button ("▼",GUILayout.Width (buttonWidth))) {
					if(i < sheet.data.Count-1) {
						Sheet1DataEntry entry = sheet.data[i];
						sheet.data[i] = sheet.data[i+1];
						sheet.data[i+1] = entry;
					}
				}
				GUILayout.EndHorizontal();	
			}
		}
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();*/


		if (GUI.changed) {
			EditorUtility.SetDirty(sheet);
		}

		//base.OnInspectorGUI ();

	}

	void Download() {
		string[] settings = AssetDatabase.FindAssets ("t:GSheetSettings");
		if (settings.Length == 0) {
			Debug.Log ("can't find settings");
			return;
		} 
		
		if (settings.Length > 1) {
			Debug.Log ("settings num > 1 error");
			return;
		} 
		
		GSheetSettings setting = AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath (settings [0]), typeof(GSheetSettings)) as GSheetSettings;
		
		GOAuth2RequestFactory requestFactory =
			new GOAuth2RequestFactory(null, "MySpreadsheetIntegration-v1", setting.GetParameters());
		SpreadsheetsService service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
		service.RequestFactory = requestFactory;
		
		SpreadsheetQuery query = new SpreadsheetQuery();
		
		query.Title = sheet.SpreadSheetName;
		query.Exact = true;
		
		//Iterate over the results
		var feed = service.Query(query);
		
		if (feed.Entries.Count == 0) {
			Debug.LogError ("can't find spreadsheet : " + sheet.SpreadSheetName);
			return;
		}
		
		
		
		SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
		WorksheetFeed wsFeed = spreadsheet.Worksheets;
		WorksheetEntry worksheet = null;
		for (int i=0; i<wsFeed.Entries.Count; i++) {
			if(wsFeed.Entries[i].Title.Text == sheet.WorkSheetName) {
				worksheet = wsFeed.Entries[i] as WorksheetEntry;
				break;
			}
		}
		if (worksheet == null) {
			Debug.LogError("can't find worksheet : " + sheet.WorkSheetName);
		}

		/*
		string title = worksheet.Title.Text;
		int rowNum = (int)worksheet.Rows;
		int colNum = (int)worksheet.Cols;
		
		Debug.Log ("Title : " + title + " Row : " + rowNum + " Col : " + colNum);
		*/

		/*
		CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
		CellFeed cellFeed = service.Query(cellQuery);
		
		// Iterate through each cell, printing its value.
		foreach (CellEntry cell in cellFeed.Entries)
		{
			Debug.Log (cell.Title.Text);
			Debug.Log (cell.Value);
		}
		*/
		AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
		
		// Fetch the list feed of the worksheet.
		ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
		ListFeed listFeed = service.Query(listQuery);

		sheet.data.Clear ();
		Type t = typeof(Sheet1DataEntry);
		BindingFlags bindingFlag = BindingFlags.NonPublic | BindingFlags.Instance;
		foreach (ListEntry row in listFeed.Entries)
		{
			object entry = Activator.CreateInstance(t);
			foreach (ListEntry.Custom element in row.Elements)
			{
				string fieldName = string.Format("_{0}",element.LocalName.ToLower());
				FieldInfo field = t.GetField(fieldName,bindingFlag);
				if(field == null) {
					Debug.LogError("null field : " + element.LocalName);
					continue;
				}
				SetValue(field,entry,element.Value);
			}
			sheet.data.Add (entry as Sheet1DataEntry);
		}
	}
	void SetValue(FieldInfo field, object entry, string value) {
		if (field.FieldType== typeof(int)) {
			field.SetValue(entry,int.Parse(value));
		} else if (field.FieldType == typeof(float)) {
			field.SetValue(entry,float.Parse(value));
		} else if (field.FieldType == typeof(string)) {
			field.SetValue(entry,value);
		} else {
			Debug.LogError("unknown type : " + field.Name);
		}
	}

				           	

	               




	[MenuItem("Assets/Create/Custom/GSheet/Create Sheet1Data")]
	public static void Create() {
		GSheetUtility.CreateAsset<Sheet1Data> ();
	}
}
