using UnityEngine;
using UnityEditor;
using System.IO;
using Google.GData.Client;
public class GSheetSettings : ScriptableObject {

	static readonly string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";
	static readonly string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

	public string CLIENT_ID;
	public string CLIENT_SECRET;

	public string ACCESS_CODE;

	public string ACCESS_TOKEN;

	public OAuth2Parameters GetParameters() {
		OAuth2Parameters parameters = new OAuth2Parameters();
		
		parameters.ClientId = CLIENT_ID;
		parameters.ClientSecret = CLIENT_SECRET;
		parameters.RedirectUri = REDIRECT_URI;
		parameters.Scope = SCOPE;
		parameters.AccessCode = ACCESS_CODE;

		parameters.AccessToken = ACCESS_TOKEN;

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
		ACCESS_TOKEN = parameters.AccessToken;

		EditorUtility.SetDirty (this);
	}
}
