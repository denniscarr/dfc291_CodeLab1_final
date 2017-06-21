using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    // LEVEL GEN VARIABLES
	public float levelSize; // The size of the level (the level is always square so this is the width and height.)
    [SerializeField] float levelSizeIncrease;   // How much the size of each level increases.

    int numberOfEnemies;
    [SerializeField] int basicEnemiesAddedPerLevel = 7;
    [SerializeField] int firstLevelWithTankEnemies = 2;
    [SerializeField] int tankEnemiesAddedPerLevel = 1;

    float numberOfObstacles;
    [SerializeField] int numberOfObstaclesMin = 10;  // The minimum number of obstacles that can appear in a level.
    [SerializeField] int numberOfObstaclesMax = 50;  // The maximum number of obstacles that can appear in a level.
    public float obstacleSizeMin;
	public float obstacleSizeMax;

    // PREFAB REFERENCES
	[SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject tankEnemyPrefab;
	[SerializeField] private GameObject obstaclePrefab;

    GameManager gameManager;
	Transform playerSpawnPoint;
    Transform player;
    [SerializeField] Transform floor;
    [SerializeField] Transform[] walls;


	private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
		playerSpawnPoint = GameObject.Find ("Player Spawn Point").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        floor = GameObject.Find("Floor").transform;
	}
		

	public void Generate ()
	{
        if (gameManager.levelNumber != 0) levelSize += levelSizeIncrease;
        numberOfEnemies = 0; 

        numberOfObstacles = Random.Range(numberOfObstaclesMin, numberOfObstaclesMax) + gameManager.levelNumber * 4;

        // Clear level of all current obstacles and enemies.
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
			Destroy (go);
		}

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
			Destroy (go);
		}

        foreach (EnemyShot shot in FindObjectsOfType<EnemyShot>())
        {
            Destroy(shot.gameObject);
        }

        SetupWallsAndFloor();
			
		// Put all things in the level.
		for (int i = 0; i < numberOfObstacles; i++) {
            PlaceObstacle();
		}

		for (int i = 0; i < gameManager.levelNumber * basicEnemiesAddedPerLevel; i++) {
            PlaceEnemy(basicEnemyPrefab);
		}

        if (gameManager.levelNumber >= firstLevelWithTankEnemies)
        {
            for (int i = 0; i < (gameManager.levelNumber - firstLevelWithTankEnemies + 1) * tankEnemiesAddedPerLevel; i++)
            {
                PlaceEnemy(tankEnemyPrefab);
            }
        }

        Debug.Log("Number of enemies: " + numberOfEnemies);
        gameManager.currentEnemyAmt = numberOfEnemies;

        // Place the player in the correct spot above the level.
        player.transform.position = new Vector3(player.transform.position.x, playerSpawnPoint.position.y, player.transform.position.z);

        // Re-enable the floor's collision (since it is disabled when the player completes a level.)
		floor.GetComponent<Collider> ().enabled = true;

        // Update billboards.
		GameObject.Find ("Game Manager").GetComponent<BatchBillboard> ().UpdateBillboards ();
	}


    public void SetupWallsAndFloor()
    {
        // Give the correct size and position to the floor and walls.
        floor.localScale = new Vector3(
            levelSize * 0.2f,
            1f,
            levelSize * 0.2f
            );

        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].localScale = new Vector3(
                levelSize * 2f,
                walls[i].localScale.y,
                walls[i].localScale.z
                );

            if (walls[i].position.z > 0) walls[i].position = new Vector3(0f, walls[i].transform.position.y, levelSize);
            else if (walls[i].position.z < 0) walls[i].position = new Vector3(0f, walls[i].transform.position.y, -levelSize);
            else if (walls[i].position.x > 0) walls[i].position = new Vector3(levelSize, walls[i].transform.position.y, 0f);
            else walls[i].position = new Vector3(-levelSize, walls[i].transform.position.y, 0f);
        }
    }


    void PlaceObstacle()
    {
        Vector3 newPosition = Vector3.zero;
        Vector3 newScale = Vector3.zero;
        Quaternion newRotation = Quaternion.identity;

        bool placed = false;
        int loopSafeguard = 0;

        while (!placed)
        {
            // Get size
            newScale = new Vector3(
                Random.Range(obstacleSizeMin, obstacleSizeMax),
                20f,
                Random.Range(obstacleSizeMin, obstacleSizeMax)
            );

            // Get my position
            newPosition = new Vector3(
                Random.Range(-levelSize + newScale.x / 2, levelSize - newScale.x / 2),
                newScale.y*0.5f,
                Random.Range(-levelSize + newScale.z / 2, levelSize - newScale.z / 2)
            );

            // Get a random rotation.
            //newRotation = Quaternion.Euler(newRotation.x, Random.Range(-180f, 180f), newRotation.y);

            // Test this location with an overlap box that is high enough to catch the player in midair.
            // Also make it a little bit larger than the actual obstacle.
            Collider[] overlaps = Physics.OverlapBox(newPosition, new Vector3(newScale.x * 1.6f, 400, newScale.z * 1.6f), newRotation);

            // Make sure this obstacle isn't going to be placed on top of the player or an enemy.
            placed = true;
            foreach (Collider collider in overlaps)
            {
                if (collider.tag == "Player" || collider.tag == "Enemy" || collider.tag == "Obstacle" || collider.tag == "Wall")
                {
                    placed = false;
                }
            }

            loopSafeguard++;
            if (loopSafeguard > 100) return;
        }

        // Instantiate the obstacle.
        GameObject newObstacle = Instantiate(obstaclePrefab);
        newObstacle.transform.position = newPosition;
        newObstacle.transform.localScale = newScale;
        newObstacle.transform.rotation = newRotation;
    }


    void PlaceEnemy(GameObject enemyToPlace)
    {
        GameObject newEnemy = Instantiate(enemyToPlace);
        Vector3 newPosition = Vector3.zero;

        bool placed = false;
        int loopSafeguard = 0;

        while (!placed)
        {
            // Get my position
            newPosition = new Vector3(
                Random.Range(-levelSize + newEnemy.GetComponent<Collider>().bounds.extents.x, levelSize - newEnemy.GetComponent<Collider>().bounds.extents.x),
                2f,
                Random.Range(-levelSize + newEnemy.GetComponent<Collider>().bounds.extents.z, levelSize - newEnemy.GetComponent<Collider>().bounds.extents.z)
            );

            // Test this location
            placed = true;
            Collider[] overlaps = Physics.OverlapSphere(newPosition, newEnemy.GetComponent<Collider>().bounds.extents.x * 1.5f);
            foreach (Collider c in overlaps)
            {
                if (c.tag == "Player" || c.tag == "Enemy" || c.tag == "Obstacle" || c.tag == "Wall")
                {
                    Debug.Log("Tried to place enemy on " + c.tag);
                    placed = false;
                }
            }

            loopSafeguard++;
            if (loopSafeguard > 100)
            {
                Debug.Log("Infinite Loop");
                return;
            }
        }

        newEnemy.transform.position = newPosition;

        numberOfEnemies++;
    }
}
