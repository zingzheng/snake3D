using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankList : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		WWW www = new WWW ("http://127.0.0.1:8000/list_top/");
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log (www.text);
			GetComponent<Text>().text = www.text.ToString();

		} else {
			Debug.Log (www.error);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
