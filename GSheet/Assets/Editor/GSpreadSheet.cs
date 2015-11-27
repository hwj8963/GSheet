using UnityEngine;
using UnityEditor;
using System.Collections;
using Google.GData.Spreadsheets;
using Google.GData.Client;

public class GSpreadSheet : ScriptableObject {

	public string SpreadSheetName;
	public string WorkSheetName;

	//[SerializeField]
	///public GSheetDataList data;

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

		query.Title = SpreadSheetName;
		query.Exact = true;

		//Iterate over the results
		var feed = service.Query(query);

		if (feed.Entries.Count == 0) {
			Debug.LogError ("can't find spreadsheet : " + SpreadSheetName);
			return;
		}



		SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
		WorksheetFeed wsFeed = spreadsheet.Worksheets;
		WorksheetEntry worksheet = null;
		for (int i=0; i<wsFeed.Entries.Count; i++) {
			if(wsFeed.Entries[i].Title.Text == WorkSheetName) {
				worksheet = wsFeed.Entries[i] as WorksheetEntry;
				break;
			}
		}
		if (worksheet == null) {
			Debug.LogError("can't find worksheet : " + WorkSheetName);
		}

		string title = worksheet.Title.Text;
		int rowNum = (int)worksheet.Rows;
		int colNum = (int)worksheet.Cols;

		Debug.Log ("Title : " + title + " Row : " + rowNum + " Col : " + colNum);


		CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
		CellFeed cellFeed = service.Query(cellQuery);
		
		// Iterate through each cell, printing its value.
		foreach (CellEntry cell in cellFeed.Entries)
		{
			Debug.Log (cell.Title.Text);
			Debug.Log (cell.Value);
		}

		AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
		
		// Fetch the list feed of the worksheet.
		ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
		ListFeed listFeed = service.Query(listQuery);
		/*
		data = new GSheetDataList ();
		// Iterate through each row, printing its cell values.
		foreach (ListEntry row in listFeed.Entries)
		{
			data.AddEntry(new GSheetDataListEntry(row));
		}*/
	}
}
