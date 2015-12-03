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

	public override void OnInspectorGUI ()
	{
		if (GUILayout.Button ("Download From Google Spread Sheet")) {
			Download();
			EditorUtility.SetDirty(sheet);
		}
		base.OnInspectorGUI ();

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
