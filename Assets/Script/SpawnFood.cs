using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour {
	// Food Prefab
	public GameObject foodPrefab;

	// Borders
	public Transform borderTop;
	public Transform borderBottom;
	public Transform borderLeft;
	public Transform borderRight;


	// Use this for initialization
	void Start () {
		InvokeRepeating("Spawn", 0.3f, 4);
	}


	void Spawn()
	{


		// x position between left & right border
		float x = (float) (int)Random.Range(borderLeft.position.x+1.0f,
			borderRight.position.x-1.0f);
		
		// y position between top & bottom border
		float z = (float) (int)Random.Range(borderBottom.position.z+1.0f,
			borderTop.position.z-1.0f);

		// Instantiate the food at (x, y)
		Instantiate(foodPrefab, new Vector3(x, 0.5f, z), Quaternion.identity); // default rotation

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
