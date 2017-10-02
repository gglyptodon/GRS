using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {
	[Serializable]
	public class Count
	{
		public int maximum;
		public int minimum;

		public Count (int min, int max){
			minimum = min;
			maximum = max;
		}
	}

	public int colums = 8;
	public int rows = 8;

	public Count wallCount = new Count (2, 4);
	public Count taxFormCount = new Count(3,5);
	public Count alcoholCount = new Count(1,3);

	public GameObject taxOffice;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] taxFormTiles;
	public GameObject[] alcoholTiles;
	public GameObject[] outerWallTiles;


	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();

	void InitializeList(){
		gridPositions.Clear ();
		for (int x = 1; x < colums - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup(){
		boardHolder = new GameObject ("Board").transform;
		for (int x = -1; x < colums + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x == -1 || x == colums || y == -1 || y == rows) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}

				GameObject instance = Instantiate(toInstantiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}

	}

	Vector3 RandomPosition(){
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){
		int objectCount = Random.Range (minimum, maximum + 1);
		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPositon = RandomPosition ();
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			Instantiate (tileChoice, randomPositon, Quaternion.identity);
		}
	
	}
	public void SetupScene(int level){
		BoardSetup ();
		InitializeList ();
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom (alcoholTiles, alcoholCount.minimum, alcoholCount.maximum);
		LayoutObjectAtRandom (taxFormTiles, taxFormCount.minimum, taxFormCount.maximum);
		//todo adjust sanity
		Instantiate(taxOffice, new Vector3(colums -1 , rows -1, 0f),Quaternion.identity);
	}
}
