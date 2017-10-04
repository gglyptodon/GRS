using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public float levelStartDelay = 3f;	
	public float turnDelay = 0.1f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerAlcoholPoints = 0;
    public int playerSanity = 100;
    public double playerFormsCollected = 0f;
	[HideInInspector] public bool playersTurn = true;

	private int level = 1;
	private List<Enemy> enemies = new List<Enemy>(); // had to init as i didnt put anything via gui
	private bool enemiesMoving;

	public AudioClip taxGroan1;
	public AudioClip taxGroan2;
	public AudioClip taxGroan3;
	public AudioClip taxGroan4;
	// Text stuff
	//load of messages
	public string[] taxErrorList;
	private int[] taxErrorSanityPenaltyList = new int[100];


	private Text taxErrorText;
	private Text winText;
	public string taxErrorPool;
/*       
	"Invalid address: please enter a valid Gondwana address.;Payments from Bank of Theropods not supported.;Missing receipts.;Taxosaur account locked due to inactivity;"; 
	taxErrorPool = taxErrorPool + "Form 190854.5e had been deprecated, use 190854.5ez instead.;";
	taxErrorPool = taxErrorPool +	"Out of ink.;";
		"Simplified version of 190854.5ez not sufficient for your tax situation, use 190854.5f instead.;\" +
		"Math error leads to taxes equaling 112% of income.;" +
    	"Leptoceraptopsids ate the tax forms.;\" +
		"Leptoceraptopsids ate the tax forms (again).;\" +
		"Payment submitted under wrong category, please pay again + late penalty.;\" +
		"Gondwana Revenue Service website undergoing maintenance.;\" +
		"Missing last years return.;\" +
		"Gondwana Revenue Service simplified the tax code, start over.;\"+
		"Math error leads to taxes equaling 209% of income.;\"+
		"A missed field leads to taxes equaling -3% of income, start over or face Allosaurus the auditor.;";
*/
	// text for level transition
	private Text lesothosaurusText; // only used at the start


	private GameObject taxErrorImage; // image for level transition
	private GameObject winImage;

	private bool doingSetup = true;

	void Start(){


	}

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);

		taxErrorPool = 	"Invalid address: please enter a valid Gondwana address.;Payments from Bank of Theropods not supported.;Missing receipts.;Taxosaur account locked due to inactivity.;"; 
		taxErrorPool = taxErrorPool +"Form 190854.5e had been deprecated, use 190854.5ez instead.;Missing last years return.;Gondwana Revenue Service simplified the tax code, start over.;";
		taxErrorPool = taxErrorPool +"Math error leads to taxes equaling 209% of income.;Simplified version of 190854.5ez not sufficient for your tax situation, use 190854.5f instead.;";
		taxErrorPool = taxErrorPool + "Math error leads to taxes equaling 112% of income.;Leptoceraptopsids ate the tax forms (again).;Payment submitted under wrong category, please pay again + late penalty.;";
		taxErrorPool = taxErrorPool +"Leptoceraptopsids ate the tax forms.;Gondwana Revenue Service website undergoing maintenance.;";
		taxErrorPool = taxErrorPool +	"Out of ink.;A missed field leads to taxes equaling -3% of income, start over or face Allosaurus the auditor.;";

		//taxErrorList = taxErrorPool.Split (";");
		taxErrorList = taxErrorPool.Split(';'); 
		for (int i =0; i<taxErrorList.Length;i++){
			taxErrorSanityPenaltyList [i] = Random.Range (0, 40); //todo check if that's a good idea 
		}
		print (taxErrorList);

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
		winText.text = "You... win (?)";
		taxErrorImage.SetActive (false);
		winImage.SetActive (true);
		enabled = false;
	}
	// Update is called once per frame
	void InitGame () {
		doingSetup = true;
		taxErrorImage = GameObject.Find ("TaxErrorImage"); // aka levelImage
		winImage = GameObject.Find("WinImage");
		lesothosaurusText = GameObject.Find("LesothosaurusText").GetComponent<Text>();
		taxErrorText = GameObject.Find("TaxErrorText").GetComponent<Text>(); // aka LevelText
		winText = GameObject.Find("WinText").GetComponent<Text>();
		if (level == 1) {
			taxErrorText.text = "It's Tax Day... ";
			lesothosaurusText.text = "...For Lesothosaurus";

		} else {
			taxErrorText.color = Color.red;
			taxErrorText.fontSize = 42;
		
			int randomerrorindex = Random.Range (0, taxErrorList.Length);
			//print (taxErrorList.Length);
			//print (randomerrorindex);
			//print ("RAND");
			taxErrorText.text = taxErrorList[randomerrorindex];
            GameManager.instance.playerSanity -= taxErrorSanityPenaltyList[randomerrorindex];

            lesothosaurusText.text = "";
			SoundManager.instance.RandomizeSfx(taxGroan1, taxGroan2, taxGroan3, taxGroan4);

		}
		taxErrorImage.SetActive (true); //display img
		winImage.SetActive(false);
		Invoke("HideTaxErrorImage", levelStartDelay); 

		enemies.Clear (); // todo remove unneccessary things from the tutorial
		boardScript.SetupScene (level);
		
	}

	void HideTaxErrorImage(){
		//Disable img
		taxErrorImage.SetActive(false);
		winImage.SetActive (false);
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
