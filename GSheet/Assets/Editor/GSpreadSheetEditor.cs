using UnityEngine;
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
		spreadSheet.WorkSheetName = EditorGUILayout.TextField ("WorkSheet Name", spreadSheet.WorkSheetName);
		if (GUILayout.Button ("Find SpreadSheet")) {
			spreadSheet.FindSpreadSheet ();
		}
		//spreadSheet.data.OnInspectorGUI ();

		if (GUI.changed) {
			EditorUtility.SetDirty(spreadSheet);
		}

	}

	[MenuItem("Custom/GSheet/Create GSpreadSheet")]
	public static void CreateSpreadSheet() {
		GSheetUtility.CreateAsset<GSpreadSheet> ();
	}

}
