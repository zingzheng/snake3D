﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;

public class playerControl : MonoBehaviour {


	public GameObject tailPrefab;
	public Action OnLose;
	public Action OnWin;
	public Action AddScore;
	public float speed=1.0f;

	private Vector3 dir;
	bool ate = false;
	List<Transform> tail = new List<Transform>();

	// Use this for initialization
	void Start () {
		dir = new Vector3(1.0f*speed,0.0f,0.0f);
		for (int i = 0; i < 5; i++) {
			GameObject g = (GameObject)Instantiate (tailPrefab, new Vector3(-(1.0f+i),0.5f,0.0f), Quaternion.identity);
			tail.Insert (i, g.transform);
		}
		InvokeRepeating ("Move", 0.3f, 0.3f);
	}

	void Move () {
		Vector3 pos = transform.position;
		transform.Translate (dir);
		if (ate) {
			//speed += 0.5f * tail.Count / 10;
			AddScore ();
			GameObject g = (GameObject)Instantiate (tailPrefab, pos, Quaternion.identity);
			tail.Insert (0, g.transform);
			ate = false;

		} else if (tail.Count > 0) {
			tail.Last ().position = pos;
			tail.Insert (0, tail.Last ());
			tail.RemoveAt (tail.Count - 1);
		}
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
			
	}

	void OnTriggerEnter(Collider coll){
		if (coll.name.StartsWith ("food")) {
			ate = true;
			Destroy (coll.gameObject);
		} else {
			OnLose ();
		}
	}
		
}
