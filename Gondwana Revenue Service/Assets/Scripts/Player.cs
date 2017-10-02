using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using UnityEngine.UI;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	public int pointsPerAlcohol = 5;              //Number of points to add to player drunkenness points when picking up a alcohol drink object.
	public int pointsPerTaxes = 5;              //Number of points to add to player food points when picking up a taxes object.
    public int pointsPerWall = 5;               // Number of points to take from drunkenness and add to sanity when smashing walls
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
	public Text playerSanityText;
	public Text playerAlcoholPointsText;

	private Animator animator;                  //Used to store a reference to the Player's animator component.
	private int drunkenness;                           //Used to store player drunkenness points total during level.
    private int sanity;                         // Stores sanity points during level
    private int newforms;                       // Stores forms collected just during level. (or will...)

	private bool isFacingRight;
	private bool isBlockedWhileMoving;


	//Start overrides the Start function of MovingObject
	protected override void Start ()
	{
		isBlockedWhileMoving = false;
		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();

		//Get the current drunkenness point total stored in GameManager.instance between levels.
		drunkenness = GameManager.instance.playerAlcoholPoints;
		//playerAlcoholPointsText = GameObject.Find ("PlayerAlcoholPointsText").GetComponent<Text>;
		playerAlcoholPointsText.text = "Drunkenness: "+drunkenness.ToString();
        //Same idea for sanity
        sanity = GameManager.instance.playerSanity;

		//playerSanityText = GameObject.Find ("PlayerSanityText").GetComponent<Text>;
		playerSanityText.text = "Sanity: " + sanity.ToString ();
		//Call the Start function of the MovingObject base class.
		isFacingRight = true;
		base.Start ();
	}


	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable ()
	{
		//When Player object is disabled, store the current local drunkenness total in the GameManager so it can be re-loaded in next level.
		GameManager.instance.playerAlcoholPoints = drunkenness;
        GameManager.instance.playerSanity = sanity;
	}


	private void FixedUpdate ()
	{
		//If it's not the player's turn, exit the function.
		if(!GameManager.instance.playersTurn) return;
		if (isBlockedWhileMoving) return;

		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.


		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
			vertical = 0;
		}

		//Check if we have a non-zero value for horizontal or vertical
		if(horizontal != 0 || vertical != 0)
		{
			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			Flip(horizontal); //added to flip the sprite
			//todo trigger animation
			animator.SetTrigger("playerIsMoving");
			AttemptMove<Wall> (horizontal, vertical);
		}
		// todo maybe just end the animation instead of checking every time
		//else if (horizontal == 0 && vertical == 0 && animator.GetBool("playerIsMoving") == true){
		//	animator.SetBool ("playerIsMoving", false);
		//}
	}
	// flip player sprite
	private void Flip(float horizontal){
		if (horizontal > 0 && !isFacingRight || horizontal < 0 && isFacingRight) {
			isFacingRight = !isFacingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
			
		}
	}


	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		//Every time player moves, subtract from drunkenness points total, add sanity.
		drunkenness--;
        sanity++;
		isBlockedWhileMoving = true;
		playerAlcoholPointsText.text = "Drunkenness: " + drunkenness.ToString ();
		playerSanityText.text = "Sanity: " + sanity.ToString ();

		//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
		base.AttemptMove <T> (xDir, yDir);

		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D hit;

		//If Move returns true, meaning Player was able to move into an empty space.
		if (Move (xDir, yDir, out hit)) 
		{
			//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
		}
		isBlockedWhileMoving = false;

		//Set the playersTurn boolean of GameManager to false now that players turn is over.
		GameManager.instance.playersTurn = false;
	}


	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
	protected override void OnCantMove <T> (T component)
	{
		//Set hitWall to equal the component passed in as a parameter.
		Wall hitWall = component as Wall;


		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger ("playerAttack");
		//Call the DamageWall function of the Wall we are hitting.
		hitWall.DamageWall (wallDamage);
        // smashing things reduced drunkenness and increases sanity
        sanity += pointsPerWall;
        drunkenness -= pointsPerWall;
		playerAlcoholPointsText.text = "Drunkenness: " + drunkenness.ToString ();
		playerSanityText.text = "Sanity: " + sanity.ToString ();

	}



	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D (Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		if(other.tag == "TaxOffice")
		{
			//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
			Invoke ("Restart", restartLevelDelay);

			//Disable the player object since level is over.
			enabled = false;
		}

		//Check if the tag of the trigger collided with is drunkenness.
		else if(other.tag == "Alcohol") //todo check!
		{
			//Add pointsPerAlcohol to the players current drunkenness total.
			drunkenness += pointsPerAlcohol;
            sanity += pointsPerAlcohol;
			//Disable the drunkenness object the player collided with.
			other.gameObject.SetActive (false);
		}

		//Check if the tag of the trigger collided with is a tax form.
		else if(other.tag == "Taxes")
		{
			//Add pointsPerTaxes to players drunkenness points total
			sanity -= pointsPerTaxes;
            drunkenness -= pointsPerTaxes;

			//Disable the taxes object the player collided with.
			other.gameObject.SetActive (false);
		}
	}


	//Restart reloads the scene when called.
	private void Restart ()
	{
		//Load the last scene loaded, in this case Main, the only scene in the game.
		SceneManager.LoadScene (0);
	}


	//LoseDrunkenness is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
    // can probably be deleted?
	public void LoseDrunkenness (int loss)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger ("playerHit");

		//Subtract lost drunkenness points from the players total.
		drunkenness -= loss;

		//Check to see if game has ended.
		CheckIfGameOver ();
	}


	//CheckIfGameOver checks if the player is out of drunkenness points and if so, ends the game.
	private void CheckIfGameOver ()
	{
		//Check if drunkenness point total is less than or equal to zero.
		if (drunkenness >= 100) 
		{
			//Call the GameOver function of GameManager.
			GameManager.instance.GameOver ();
		}
        else if (sanity <= 0)
        {
            GameManager.instance.GameOver();
        }
    }







}