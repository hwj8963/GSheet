using UnityEngine;
using UnityEditor;
using System.Collections;
using Google.GData.Spreadsheets;
using Google.GData.Client;

public class GSpreadSheet : ScriptableObject {

	public string SpreadSheetName;
	public string SpreadSheetKey;

	public void FindSpreadSheet() {
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
		query.Title = "Exact Title";
		query.Exact = true;
		
		//Iterate over the results
		var feed = service.Query(query);
		foreach (var entry in feed.Entries)
		{
			Debug.Log (entry.Id + ":"+ entry.Title.Text);
		}
	}
}
