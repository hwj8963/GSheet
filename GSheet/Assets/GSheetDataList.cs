using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Google.GData.Spreadsheets;
/*

[System.Serializable]
public class GSheetDataListEntry {

	[SerializeField]
	public Dictionary<string,object> vals;
	public GSheetDataListEntry(ListEntry entry) {
		vals = new Dictionary<string,object> ();
		foreach (ListEntry.Custom element in entry.Elements)
		{
			vals.Add (element.LocalName,GetValue (element.LocalName,element.Value));
		}
	}

	public object GetValue(string key, string value) {
		if (key [0] == 'n') {
			return int.Parse(value);
		} else if (key [0] == 'f') {
			return float.Parse (value);
		} else if (key [0] == 's') {
			return value;
		} else {
			return value;
		}
	}	
	public void OnInspectorGUI() {
		string chanedKey = "";
		List<string> keys = new List<string>(vals.Keys);
		for (int i=0; i<keys.Count; i++) {
			string key = keys[i];
			if (key [0] == 'n') {
				vals[key] = EditorGUILayout.IntField(key,(int)vals[key]);
			} else if (key [0] == 'f') {
				vals[key] = EditorGUILayout.FloatField(key,(float)vals[key]);
			} else if (key [0] == 's') {
				vals[key] = EditorGUILayout.TextField(key,(string)vals[key]);
			} else {
				vals[key] = EditorGUILayout.TextField(key,(string)vals[key]);
			}
		}
	}
}


[System.Serializable]
public class GSheetDataList {
	[SerializeField]
	public List<GSheetDataListEntry> data = new List<GSheetDataListEntry>();

	public void AddEntry(GSheetDataListEntry entry) {
		data.Add (entry);
	}


	public void OnInspectorGUI() {

		for(int i=0;i<data.Count;i++) {
			EditorGUILayout.Separator ();
			data[i].OnInspectorGUI();

		}

	}
}*/
