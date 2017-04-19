using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	// Used for moving
	public float moveRandomness = 40f;
	public float moveDistanceMin = 1f;
	public float moveDistanceMax = 15f;
	public float moveSpeed = 5f;
	Transform playerTransform;
	Vector3 targetPosition;


	// Used for shooting
	public float shotTimerMin = 0.7f;
	public float shotTimerMax = 5f;
	public float preShotDelay = 0.7f;
	public float postShotDelay = 0.4f;
	public GameObject shotPrefab;
	Timer shotTimer;


	// Used for getting hurt
	public int HP = 20;
	public GameObject deathParticles;


	// Used for material modification
	Material material;
	float noiseTime;
	public float noiseSpeed = 0.01f;
	float noiseRange = 100f;


	// Behavior states
	static int PREPARING_TO_MOVE = 0;
	static int MOVING = 1;
	static int PRE_SHOOTING = 2;
	static int SHOOTING = 3;
	static int POST_SHOOTING = 4;
	int currentState;

    ScoreControllerScript scoreControllerScript;
	Rigidbody rb;
	Animator animator;
    //public GameObject findHelper;

	bool alive = true;


	void Start()
	{
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;

		currentState = PREPARING_TO_MOVE;

		rb = GetComponent<Rigidbody> ();

        scoreControllerScript = GameObject.Find("Score Display").GetComponent<ScoreControllerScript>();

		animator = GetComponent<Animator> ();
		animator.speed = 1.0f / preShotDelay;

		material = GetComponentInChildren<MeshRenderer> ().material;
		noiseTime = Random.Range (-1000f, 1000f);

		shotTimer = new Timer (Random.Range(shotTimerMin, shotTimerMax));
	}


	void Update ()
	{
		// Change material tiling
		material.mainTextureScale = new Vector2(MyMath.Map(Mathf.PerlinNoise(noiseTime, 0), 0f, 1f, -noiseRange, noiseRange), 0);
		noiseTime += noiseSpeed;

		if (currentState == PREPARING_TO_MOVE) {
			PrepareToMove ();
		} else if (currentState == MOVING) {
			Move ();
		} else if (currentState == PRE_SHOOTING) {
			PreShoot ();
		} else if (currentState == SHOOTING) {
			Shoot ();
		} else if (currentState == POST_SHOOTING) {
			PostShoot ();
		}
	}


	void PrepareToMove()
	{
		// Get a random point in the vicinity of the player
		Vector3 nearPlayer = playerTransform.position + Random.insideUnitSphere*moveRandomness;
		nearPlayer.y = transform.position.y;

		// Get a direction to that point
		Vector3 direction = nearPlayer - transform.position;
		direction.Normalize ();

		// Scale that direction to a random magnitude
		targetPosition =  transform.position + direction * Random.Range(moveDistanceMin, moveDistanceMax);

		targetPosition.y = transform.position.y;

		currentState = MOVING;
	}


	void Move()
	{
		// See if it's time to shoot at the player
		shotTimer.Run();
		if (shotTimer.finished)
		{
			// Set timer for pre shot delay
			shotTimer = new Timer (preShotDelay);
			animator.SetTrigger ("ChargeUp");
			currentState = PRE_SHOOTING;

			return;
		}

		// Move towards target position
		Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed*Time.deltaTime);

		// If we've reached the target position, find a new target position
		if (newPosition == targetPosition) {
			currentState = PREPARING_TO_MOVE;
		}

		rb.MovePosition (newPosition);
	}


	void PreShoot() {
		shotTimer.Run ();
		if (shotTimer.finished) {
			currentState = SHOOTING;
		}
	}

	void Shoot() {
		Instantiate (shotPrefab, transform.position, Quaternion.identity);
		shotTimer = new Timer (postShotDelay);
		currentState = POST_SHOOTING;
	}

	void PostShoot() {
		shotTimer.Run ();
		if (shotTimer.finished) {
			shotTimer = new Timer (Random.Range (shotTimerMin, shotTimerMax));
			currentState = PREPARING_TO_MOVE;
		}
	}

	void OnCollisionEnter(Collision collision) {
	
		if (collision.collider.tag == "Obstacle" || collision.collider.tag == "Wall" || collision.collider.tag == "Enemy") {
			currentState = PREPARING_TO_MOVE;
		}
	}

	public void GetHurt() {
		HP -= 1;
		if (HP <= 0) {
			Die ();
		}
	}


	void Die()
    {
		if (alive)
        {
			Instantiate (deathParticles, transform.position, Quaternion.identity);
			Destroy (gameObject);
            scoreControllerScript.KilledEnemy();
			alive = false;
		}
	}
}
