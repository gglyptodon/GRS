﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public float turnDelay = .1f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerAlcoholPoints = 0;
    public int playerSanity = 100;
	[HideInInspector] public bool playersTurn = true;

	private int level = 3;
	private List<Enemy> enemies = new List<Enemy>(); // had to init as i didnt put anything via gui
	private bool enemiesMoving;

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
		boardScript = GetComponent<BoardManager>();
		InitGame ();
	}

       //Update is called every frame.
        void Update()
        {
            //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
            if(playersTurn || enemiesMoving)
                
                //If any of these are true, return and do not start MoveEnemies.
                return;
            
            //Start moving enemies.
            StartCoroutine (MoveEnemies ());
        }

 //Call this to add the passed in Enemy to the List of Enemy objects.
        public void AddEnemyToList(Enemy script)
        {
            //Add Enemy to List enemies.
            enemies.Add(script);
        }
	
	public void GameOver (){
		enabled = false;
	}
	// Update is called once per frame
	void InitGame () {
		enemies.Clear ();
		boardScript.SetupScene (level);
		
	}

	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}
		//Loop through List of Enemy objects.
            for (int i = 0; i < enemies.Count; i++){
                //Call the MoveEnemy function of Enemy at index i in the enemies List.
                enemies[i].MoveEnemy ();
                
                //Wait for Enemy's moveTime before moving next Enemy, 
                yield return new WaitForSeconds(enemies[i].moveTime);
            }
            //Once Enemies are done moving, set playersTurn to true so player can move.
            playersTurn = true;
            
            //Enemies are done moving, set enemiesMoving to false.
            enemiesMoving = false;
	}
}
