using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;

public class playerControl : MonoBehaviour {


	public GameObject tailPrefab;
	public GameObject tailPrefab1;
	public GameObject tailPrefab2;
	public GameObject tailPrefab3;
	public GameObject tailPrefab4;
	public GameObject tailPrefab5;
	public GameObject tailPrefab6;
	public Action OnLose;
	public Action OnWin;
	public Action AddScore;
	public Action AddBScore;
	public float speed=1.0f;

	private Vector3 dir;
	bool ate = false;
	bool check = false;
	List<Transform> tail = new List<Transform>();
	int type;
	private List<GameObject> tailPrefabs = new List<GameObject>();

	// Use this for initialization
	void Start () {
		tailPrefabs.Add (tailPrefab1);
		tailPrefabs.Add (tailPrefab2);
		tailPrefabs.Add (tailPrefab3);
		tailPrefabs.Add (tailPrefab4);
		tailPrefabs.Add (tailPrefab5);
		tailPrefabs.Add (tailPrefab6);
		dir = new Vector3(1.0f*speed,0.0f,0.0f);
//		for (int i = 0; i < 5; i++) {
//			GameObject g = (GameObject)Instantiate (tailPrefab, new Vector3(-(1.0f+i),0.5f,0.0f), Quaternion.identity);
//			tail.Insert (i, g.transform);
//		}
		InvokeRepeating ("Move", 0.3f, 0.3f);
	}

	void Move () {
		Vector3 pos = transform.position;
		transform.Translate (dir);
		for(int i =0; i<tail.Count;i++){
			Vector3 tmp = tail [i].position;
			tail [i].position = pos;
			pos = tmp;
		}
		if (ate) {
			//speed += 0.5f * tail.Count / 10;
			AddScore ();
			GameObject g = (GameObject)Instantiate (tailPrefabs[type-1], pos, Quaternion.identity);
			tail.Add (g.transform);
			ate = false;
			check = true;

		} 
//		else if (tail.Count > 0) {
//			tail.Last ().position = pos;
//			tail.Insert (0, tail.Last ());
//			tail.RemoveAt (tail.Count - 1);
//		}
	}

	// Update is called once per frame
	void Update () {

		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) {
			// Get movement of the finger since last frame
			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
			if (Math.Abs (touchDeltaPosition.x) > Math.Abs (touchDeltaPosition.y)) {
				if (touchDeltaPosition.x > 0 && dir.x>=0.0f) {
					dir = new Vector3 (1.0f * speed, 0.0f, 0.0f);
				} 
				if (touchDeltaPosition.x < 0 && dir.x<=0.0f){
					dir = new Vector3 (-1.0f * speed, 0.0f, 0.0f);
				}
			} else {
				if (touchDeltaPosition.y > 0 && dir.z>=0.0f) {
					dir = new Vector3 (0.0f, 0.0f, 1.0f * speed);
				} 
				if (touchDeltaPosition.y < 0 && dir.z<=0.0f) {
					dir = new Vector3 (0.0f, 0.0f, -1.0f * speed);
				}
			}
		}

		if (Input.GetKey (KeyCode.D) && dir.x>=0.0f)
			dir = new Vector3(1.0f*speed,0.0f,0.0f);
		else if (Input.GetKey (KeyCode.A) && dir.x<=0.0f)
			dir = new Vector3(-1.0f*speed,0.0f,0.0f);
		else if (Input.GetKey (KeyCode.W) && dir.z>=0.0f)
			dir = new Vector3(0.0f,0.0f,1.0f*speed);
		else if (Input.GetKey (KeyCode.S) && dir.z<=0.0f)
			dir = new Vector3(0.0f,0.0f,-1.0f*speed);

		//检查连击
		if (check) {
			check = false;
			int len = tail.Count;
			if (len >= 3) {
				if (tail [len - 1].name == tail [len - 2].name &&
				   tail [len - 2].name == tail [len - 3].name) {
					Destroy (tail [len - 1].gameObject);
					Destroy (tail [len - 2].gameObject);
					Destroy (tail [len - 3].gameObject);
					tail.RemoveAt (len - 1);
					tail.RemoveAt (len - 2);
					tail.RemoveAt (len - 3);
					AddBScore ();
				}
			}
		}
			
	}

	void OnTriggerEnter(Collider coll){
		if (coll.name.StartsWith ("food")) {
			ate = true;
			if (coll.name.StartsWith ("food1")) {
				type = 1;
			}else if(coll.name.StartsWith ("food2")) {
				type = 2;
			}else if(coll.name.StartsWith ("food3")) {
				type = 3;
			}else if(coll.name.StartsWith ("food4")) {
				type = 4;
			}else if(coll.name.StartsWith ("food5")) {
				type = 5;
			}else if(coll.name.StartsWith ("food6")) {
				type = 6;
			}
			Destroy (coll.gameObject);
		} else {
			OnLose ();
		}
	}
		
}
