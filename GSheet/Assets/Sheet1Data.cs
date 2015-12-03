using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sheet1DataEntry {

	[SerializeField]
	string _sname;
	public string sName {
		get { return _sname;}
	}

	[SerializeField]
	int _nage;
	public int nAge {
		get { return _nage;}
	}

	[SerializeField]
	int _ngender;
	public int nGender {
		get { return _ngender;}
	}

	[SerializeField]
	int	 _nscore;
	public int nScore {
		get { return _nscore;}
	}
}
public class Sheet1Data : ScriptableObject{
	public string SpreadSheetName;
	public string WorkSheetName;
	public List<Sheet1DataEntry> data;
}