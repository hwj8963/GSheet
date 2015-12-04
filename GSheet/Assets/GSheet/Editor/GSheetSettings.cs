using UnityEngine;
using UnityEditor;
using System.IO;
using Google.GData.Client;
using Google.GData.Spreadsheets;
public class GSheetSettings : ScriptableObject {

	static readonly string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";
	static readonly string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

	public string CLIENT_ID;
	public string CLIENT_SECRET;

	public string ACCESS_CODE;

	public string ACCESS_TOKEN;
	public string REFRESH_TOKEN;

	OAuth2Parameters GetParameters() {
		OAuth2Parameters parameters = new OAuth2Parameters();
		
		parameters.ClientId = CLIENT_ID;
		parameters.ClientSecret = CLIENT_SECRET;
		parameters.RedirectUri = REDIRECT_URI;
		parameters.Scope = SCOPE;
		parameters.AccessCode = ACCESS_CODE;

		parameters.AccessToken = ACCESS_TOKEN;
		parameters.RefreshToken = REFRESH_TOKEN;

		return parameters; 
	}

	public void GetAccessCode() {
		OAuth2Parameters parameters = new OAuth2Parameters();
		
		parameters.ClientId = CLIENT_ID;
		parameters.ClientSecret = CLIENT_SECRET;
		parameters.RedirectUri = REDIRECT_URI;
		parameters.Scope = SCOPE;

		string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);


		Application.OpenURL (authorizationUrl);
	}


	public void GetAccessToken() {
		OAuth2Parameters parameters = new OAuth2Parameters();
		
		parameters.ClientId = CLIENT_ID;
		parameters.ClientSecret = CLIENT_SECRET;
		parameters.RedirectUri = REDIRECT_URI;
		parameters.Scope = SCOPE;
		parameters.AccessCode = ACCESS_CODE;

		OAuthUtil.GetAccessToken(parameters);
		OAuthUtil.RefreshAccessToken (parameters);

		ACCESS_TOKEN = parameters.AccessToken;
		REFRESH_TOKEN = parameters.RefreshToken;

		EditorUtility.SetDirty (this);
	}
	public SpreadsheetsService GetService() {
		GOAuth2RequestFactory requestFactory =
			new GOAuth2RequestFactory(null, "MySpreadsheetIntegration-v1", GetParameters());
		SpreadsheetsService service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
		service.RequestFactory = requestFactory;
		return service;
	}

	public WorksheetEntry GetWorkSheet(SpreadsheetsService service, string spreadSheetName, string workSheetName) {
		if (service == null) {
			return null;
		}
		SpreadsheetQuery query = new SpreadsheetQuery();
		
		query.Title = spreadSheetName;
		query.Exact = true;
		
		//Iterate over the results
		var feed = service.Query(query);
		
		if (feed.Entries.Count == 0) {
			Debug.LogError ("can't find spreadsheet : " + spreadSheetName);
			return null;
		}

		SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
		WorksheetFeed wsFeed = spreadsheet.Worksheets;
		WorksheetEntry worksheet = null;
		for (int i=0; i<wsFeed.Entries.Count; i++) {
			if(wsFeed.Entries[i].Title.Text == workSheetName) {
				worksheet = wsFeed.Entries[i] as WorksheetEntry;
				break;
			}
		}
		if (worksheet == null) {
			Debug.LogError("can't find worksheet : " + workSheetName);
		}
		return worksheet;
	}




	public string ScriptPath;
	public string EditorScriptPath;
	public string DataAssetPath;
	
	public TextAsset FieldTemplate;
	public TextAsset DataTemplate;
	public TextAsset DataEditorTemplate;
}
