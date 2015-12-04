using UnityEngine;
using UnityEditor;
using System.Collections;
using Google.GData.Spreadsheets;
using Google.GData.Client;

using System;
using System.Reflection;

public class GSheetDataEditor<Data,Entry> : Editor
	where Data : GSheetData<Entry>
	where Entry : GSheetDataEntry
{
	Data sheet;
	void OnEnable() {
		sheet = target as Data;
	}
	
	static readonly float buttonWidth = 25f;
	static readonly float fieldMinWidth = 20f;

	void PropertyField(object entry, PropertyInfo property) {
		if (property.PropertyType == typeof(int)) {
			int valueNow = (int)property.GetValue (entry,null);
			int valueNew = EditorGUILayout.IntField(valueNow,GUILayout.MinWidth(fieldMinWidth));
			if(valueNew != valueNow) {
				property.SetValue (entry,valueNew,null);
			}
		} else if (property.PropertyType == typeof(float)) {
			float valueNow = (float)property.GetValue (entry,null);
			float valueNew = EditorGUILayout.FloatField(valueNow,GUILayout.MinWidth(fieldMinWidth));
			if(valueNew != valueNow) {
				property.SetValue (entry,valueNew,null);
			}
		} else if (property.PropertyType == typeof(string)) {
			string valueNow = (string)property.GetValue (entry,null);
			string valueNew = EditorGUILayout.TextField(valueNow,GUILayout.MinWidth(fieldMinWidth));
			if(valueNew != valueNow) {
				property.SetValue (entry,valueNew,null);
			}
		} else {
			Debug.LogError("unknown type : " + property.Name);
		}
	}
	public override void OnInspectorGUI ()
	{
		if (GUILayout.Button ("Download From Google Spread Sheet")) {
			Download();
			EditorUtility.SetDirty(sheet);
		}

		sheet.SpreadSheetName = EditorGUILayout.TextField ("Spread Sheet Name", sheet.SpreadSheetName);
		sheet.WorkSheetName = EditorGUILayout.TextField ("Work Sheet Name", sheet.WorkSheetName);
		
		GUILayout.BeginHorizontal ();
		Type EntryType = typeof(Entry);
		PropertyInfo[] properties = EntryType.GetProperties ();
		for (int p=0; p<properties.Length; p++) {
			GUILayout.Label(properties[p].Name,GUILayout.MinWidth(fieldMinWidth));
		}
		GUILayout.Space (4 * buttonWidth + 16);
		GUILayout.EndHorizontal ();

		IList list = sheet.data;
		for (int i=0; i<list.Count; i++) {
			GUILayout.BeginHorizontal();
			object entry = list[i];
			for(int p=0;p<properties.Length;p++) {
				PropertyField (entry,properties[p]);
			}
			
			if(GUILayout.Button ("+",GUILayout.Width(buttonWidth))) {
				list.Insert(i,Activator.CreateInstance(EntryType));
			} 
			if(GUILayout.Button ("-",GUILayout.Width(buttonWidth))) {
				list.RemoveAt(i);
			} 
			if(GUILayout.Button ("▲",GUILayout.Width (buttonWidth))) {
				if(i > 0) {
					list[i] = list[i-1];
					list[i-1] = entry;
				}
			} 
			if(GUILayout.Button ("▼",GUILayout.Width (buttonWidth))) {
				if(i < list.Count-1) {
					list[i] = list[i+1];
					list[i+1] = entry;
				}
			}
			
			GUILayout.EndHorizontal();
		}
		if (GUI.changed) {
			EditorUtility.SetDirty(sheet);
		}

	}
	
	void Download() {
		GSheetSettings setting = GSheetUtility.GetSettings ();
		if (setting == null)
			return;
		SpreadsheetsService service = setting.GetService ();

		WorksheetEntry worksheet = setting.GetWorkSheet (service, sheet.SpreadSheetName, sheet.WorkSheetName);

		AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
		
		// Fetch the list feed of the worksheet.
		ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
		ListFeed listFeed = service.Query(listQuery);
		
		sheet.data.Clear ();
		Type t = typeof(Entry);
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
			sheet.data.Add (entry as Entry);
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
}
