using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	// USED FOR MOVING
	[SerializeField] private float moveRandomness = 40f;    // When I move, I move towards a spot inside a circle of this diameter surrounding the player.
	[SerializeField] private float moveDistanceMin = 1f;  // The shortest distance I will move.
	[SerializeField] private float moveDistanceMax = 15f; // The longest distance I will move.
	[SerializeField] private float moveSpeed = 5f;  // How quickly I move.
	private Vector3 targetPosition; // The position I am currently moving towards.

	// USED FOR SHOOTING
	[SerializeField] private float shotTimerMin = 0.7f;   // The minimum amount of time in between shots.
	[SerializeField] private float shotTimerMax = 5f;   // The maximum amount of time in between shots.
	[SerializeField] private float preShotDelay = 0.7f; // How long I pause motionless before firing a shot.
	[SerializeField] private float postShotDelay = 0.4f;    // How long I pause motionless after firing a shot.
	[SerializeField] private GameObject shotPrefab;
	private Timer shotTimer;    // Keeps track of how long it's been since I last fired a shot.

	// USED FOR GETTING HURT
	[SerializeField] private int _HP = 20; // Health points.
    public int HP
    {
        get
        {
            return _HP;
        }

        set
        {
            // See if I should die.
            if (value <= 0)
            {
                Die();
            }

            else
            {
                _HP = value;
            }
        }
    }
	[SerializeField] private GameObject deathParticles;
    private bool isAlive = true;


    // USED FOR MATERIAL MODIFICATION
    private Material myMaterial;
	private float noiseTime;
	[SerializeField] private float noiseSpeed = 0.01f;
	private float noiseRange = 100f;

	// BEHAVIOR STATES
    private enum BehaviorState { PreparingToMove, Moving, PreShooting, Shooting, PostShooting };
    private BehaviorState currentState;

    // REFERENCES
    GameManager gameManager;
	Rigidbody myRigidbody;
	Animator myAnimator;
    Transform playerTransform; 


    private void Start()
	{
        // Store miscellaneous references.
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		myRigidbody = GetComponent<Rigidbody> ();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        myAnimator = GetComponent<Animator> ();
        myMaterial = GetComponentInChildren<MeshRenderer>().material;

        // Not sure why I'm doing this - look into it later. (Does it have to do with the enemies firing shots at the wrong point during their animation?)
        myAnimator.speed = 1.0f / preShotDelay;

        // Get a random starting point for Perlin noise.
		noiseTime = Random.Range (-1000f, 1000f);

        // Get a random time to fire the next shot.
		shotTimer = new Timer (Random.Range(shotTimerMin, shotTimerMax));

        currentState = BehaviorState.PreparingToMove;
    }


    void Update ()
	{
		// Change material tiling using Perlin noise. (For the static effect).
		myMaterial.mainTextureScale = new Vector2(MyMath.Map(Mathf.PerlinNoise(noiseTime, 0), 0f, 1f, -noiseRange, noiseRange), 0);
		noiseTime += noiseSpeed;

        // Perform actions according to current BehaviorState
		if (currentState == BehaviorState.PreparingToMove)
        {
			PrepareToMove ();
		}

        else if (currentState == BehaviorState.Moving)
        {
			Move ();
		}

        else if (currentState == BehaviorState.PreShooting)
        {
			PreShoot ();
		}

        else if (currentState == BehaviorState.Shooting)
        {
			Shoot ();
		}

        else if (currentState == BehaviorState.PostShooting)
        {
			PostShoot ();
		}
	}


	void PrepareToMove()
	{
		// Get a random point in a circle around the player.
		Vector3 nearPlayer = playerTransform.position + Random.insideUnitSphere*moveRandomness;
		nearPlayer.y = transform.position.y;

		// Get a direction to that point
		Vector3 direction = nearPlayer - transform.position;
		direction.Normalize ();

		// Scale that direction to a random magnitude
		targetPosition =  transform.position + direction * Random.Range(moveDistanceMin, moveDistanceMax);

		targetPosition.y = transform.position.y;

		currentState = BehaviorState.Moving;
	}


	void Move()
	{
		// See if it's time to shoot at the player
		shotTimer.Run();
		if (shotTimer.finished)
		{
			// Set timer for pre shot delay
			shotTimer = new Timer (preShotDelay);

            // Begin the charging up animation.
			myAnimator.SetTrigger ("ChargeUp");

			currentState = BehaviorState.PreShooting;

			return;
		}

		// Move towards target position
		Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed*Time.deltaTime);
        myRigidbody.MovePosition(newPosition);

        // If we've reached the target position, find a new target position
        if (newPosition == targetPosition)
        {
			currentState = BehaviorState.PreparingToMove;
            return;
		}
	}


	void PreShoot()
    {
        // Do nothing and wait for charge-up animation to finish.
	}


	void Shoot()
    {
        // Fire a shot.
		Instantiate (shotPrefab, transform.position, Quaternion.identity);

        // Set the shot timer for the post shot delay.
		shotTimer = new Timer (postShotDelay);

		currentState = BehaviorState.PostShooting;
	}


	void PostShoot()
    {
        // Se if we've waited long enough.
		shotTimer.Run ();
		if (shotTimer.finished)
        {
            // Determite how long until the next bullet is fired.
			shotTimer = new Timer (Random.Range (shotTimerMin, shotTimerMax));

			currentState = BehaviorState.PreparingToMove;
		}
	}


    void OnCollisionEnter(Collision collision)
    {
        // If I hit a non-lethal obstacle, move to a new spot (crude pathfinding).
		if (collision.collider.tag == "Obstacle" || collision.collider.tag == "Wall" || collision.collider.tag == "Enemy")
        {
			currentState = BehaviorState.PreparingToMove;
		}
	}


	void Die()
    {
        // isAlive is used to make sure that this function is not called more than once.
		if (isAlive)
        {
			Instantiate (deathParticles, transform.position, Quaternion.identity);
            gameManager.KilledEnemy();
			isAlive = false;
            Destroy(gameObject);
        }
    }


    void ChargeUpAnimationFinished()
    {
        currentState = BehaviorState.Shooting;
    }
}
