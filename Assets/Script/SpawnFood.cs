using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour {
	// Food Prefab
	public GameObject foodPrefab;
	public GameObject foodPrefab1;
	public GameObject foodPrefab2;
	public GameObject foodPrefab3;
	public GameObject foodPrefab4;
	public GameObject foodPrefab5;
	public GameObject foodPrefab6;
	private List<GameObject> foods = new List<GameObject>();
	// Borders
	public Transform borderTop;
	public Transform borderBottom;
	public Transform borderLeft;
	public Transform borderRight;


	// Use this for initialization
	void Start () {
		foods.Add (foodPrefab1);
		foods.Add (foodPrefab2);
		foods.Add (foodPrefab3);
		foods.Add (foodPrefab4);
		foods.Add (foodPrefab5);
		foods.Add (foodPrefab6);
		InvokeRepeating("Spawn", 0.3f, 4);
	}


	void Spawn()
	{
		int type = (int)Random.Range (0, 6);


		// x position between left & right border
		float x = (float) ((int)Random.Range(borderLeft.position.x+1.0f,
			borderRight.position.x-1.0f)/2*2);
		
		// y position between top & bottom border
		float z = (float) ((int)Random.Range(borderBottom.position.z+1.0f,
			borderTop.position.z-1.0f)/2*2);

		// Instantiate the food at (x, y)
		Instantiate(foods[type], new Vector3(x, 1.0f, z), Quaternion.identity); // default rotation

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
