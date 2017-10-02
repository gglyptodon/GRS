using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public float levelStartDelay = 10f;	
	public float turnDelay = .1f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerAlcoholPoints = 0;
    public int playerSanity = 100;
	[HideInInspector] public bool playersTurn = true;

	private int level = 1;
	private List<Enemy> enemies = new List<Enemy>(); // had to init as i didnt put anything via gui
	private bool enemiesMoving;


	// Text stuff

	private Text taxErrorText; // text for level transition
	private Text lesothosaurusText; // only used at the start


	private GameObject taxErrorImage; // image for level transition

	private bool doingSetup = true;

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

	void OnLevelWasLoaded(int index){
		level++;
		InitGame ();
	
	}

       //Update is called every frame.
        void Update()
        {
            //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if(playersTurn || enemiesMoving || doingSetup)
                
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
		taxErrorText.text = "gameover";
		taxErrorImage.SetActive (true);
		enabled = false;
	}
	// Update is called once per frame
	void InitGame () {
		doingSetup = true;
		taxErrorImage = GameObject.Find ("TaxErrorImage"); // aka levelImage
		lesothosaurusText = GameObject.Find("LesothosaurusText").GetComponent<Text>();
		taxErrorText = GameObject.Find("TaxErrorText").GetComponent<Text>(); // aka LevelText
		if (level == 1) {
			taxErrorText.text = "It's Tax Day... ";
			lesothosaurusText.text = "...For Lesothosaurus";

		} else {
			taxErrorText.color = Color.red;
			taxErrorText.text = "TAX ERRORO todo";

			lesothosaurusText.text = "";
		
		}
		taxErrorImage.SetActive (true); //display img
		Invoke("HideTaxErrorImage", levelStartDelay); 

		enemies.Clear (); // todo remove unneccessary things from the tutorial
		boardScript.SetupScene (level);
		
	}

	void HideTaxErrorImage(){
		//Disable img
		taxErrorImage.SetActive(false);
		doingSetup = false;
		
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
