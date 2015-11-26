using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Google.GData.Spreadsheets;
using Google.GData.Client;
using Google.GData.Extensions;


public class GSheet {

	//https://developers.google.com/google-apps/spreadsheets/authorize
	//https://console.developers.google.com
	static readonly string CLIENT_ID = "1025324599733-l2rtceh4dn4jelkdscmbv187ug84futr.apps.googleusercontent.com";
	static readonly string CLIENT_SECRET = "KTlRkHWtwN0bq86m2n4Jo-jF";
	static readonly string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";
	static readonly string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

	[MenuItem("Custom/GSheet/Test")]
	public static void Test() {
		OAuth2Parameters parameters = new OAuth2Parameters();

		parameters.ClientId = CLIENT_ID;
		parameters.ClientSecret = CLIENT_SECRET;
		parameters.RedirectUri = REDIRECT_URI;
		parameters.Scope = SCOPE;


		string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);

		Debug.Log (authorizationUrl);

		parameters.AccessCode = "4/TcgoETSbt2BLzgl2BSxCK6HcL2-NFhSx933FADSPNZ0";


		/*OAuthUtil.GetAccessToken(parameters);
		string accessToken = parameters.AccessToken;
		Debug.Log ("OAuth Access Token: " + accessToken);
*/
		parameters.AccessToken = "ya29.OAIrJ8m9q5a7oZCNsFF4ghONF61vAuBlMOUC_74Qsytz63JuphNBm0hScG_Z43JBxT-T";

		GOAuth2RequestFactory requestFactory =
			new GOAuth2RequestFactory(null, "MySpreadsheetIntegration-v1", parameters);
		SpreadsheetsService service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
		service.RequestFactory = requestFactory;


		SpreadsheetQuery query = new SpreadsheetQuery();
		
		// Make a request to the API and get all spreadsheets.
		SpreadsheetFeed feed = service.Query(query);

		SpreadsheetEntry selectedSpreadSheet = null;
		// Iterate through all of the spreadsheets returned
		foreach (SpreadsheetEntry entry in feed.Entries) {
			if(entry.Title.Text == "UnityTest") {
				selectedSpreadSheet = entry;
				break;
			}
		}
		if (selectedSpreadSheet != null) {
			WorksheetFeed wsFeed = selectedSpreadSheet.Worksheets;
			WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];
			
			// Define the URL to request the list feed of the worksheet.
			AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
			
			// Fetch the list feed of the worksheet.
			ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
			ListFeed listFeed = service.Query(listQuery);
			
			// Iterate through each row, printing its cell values.

			foreach (ListEntry row in listFeed.Entries)
			{

				// Print the first column's cell value
				Debug.Log("TITLE" + row.Title.Text);
				// Iterate over the remaining columns, and print each cell value
				foreach (ListEntry.Custom element in row.Elements)
				{

					Debug.Log("VALUE" + element.Value + " " +element.LocalName);
				}
			}


		}
	}
}
