using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheet1Data {
	[SerializeField]
	public string sName {
		get;
		private set;
	}
	[SerializeField]
	public int nAge {
		get;
		private set;
	}
	[SerializeField]
	public int nGender {
		get;
		private set;
	}
	[SerializeField]
	public int nScore {
		get;
		private set;
	}
}
public class Sheet1Sheet : ScriptableObject{
}