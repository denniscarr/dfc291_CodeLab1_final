﻿using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    // LEVEL GEN VARIABLES
	public float levelSize; // The size of the level (the level is always square so this is the width and height.)
	float numberOfEnemies;
    [SerializeField] int enemiesAddedPerLevel = 7;
	float numberOfObstacles;
    [SerializeField] int numberOfObstaclesMin = 10;  // The minimum number of obstacles that can appear in a level.
    [SerializeField] int numberOfObstaclesMax = 50;  // The maximum number of obstacles that can appear in a level.
    public float obstacleSizeMin;
	public float obstacleSizeMax;

    // MISC REFERENCES
	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private GameObject obstaclePrefab;

	Transform playerSpawnPoint;
    Transform player;
    Transform floor;


	private void Awake()
    {
		playerSpawnPoint = GameObject.Find ("Player Spawn Point").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        floor = GameObject.Find("Floor").transform;
	}
		

	public void Generate ()
	{
        numberOfEnemies = GameObject.Find("Game Manager").GetComponent<GameManager>().levelNumber * enemiesAddedPerLevel;
        numberOfObstacles = Random.Range(numberOfObstaclesMin, numberOfObstaclesMax);

        // Clear level of all current obstacles and enemies.
        GameObject[] stuffToDelete = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject go in stuffToDelete)
        {
			Destroy (go);
		}

		stuffToDelete = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach (GameObject go in stuffToDelete)
        {
			Destroy (go);
		}
			
		// Put all things in the level.
		for (int i = 0; i < numberOfObstacles; i++) {
            PlaceObstacle();
		}

		for (int i = 0; i < numberOfEnemies; i++) {
            PlaceEnemy();
		}

        // Place the player in the correct spot above the level.
		player.transform.position = new Vector3(player.transform.position.x, playerSpawnPoint.position.y, player.transform.position.z);

        // Re-enable the floor's collision (since it is disabled when the player completes a level.)
		floor.GetComponent<MeshCollider> ().enabled = true;

        // Update billboards.
		GameObject.Find ("Game Manager").GetComponent<BatchBillboard> ().UpdateBillboards ();
	}


    void PlaceObstacle()
    {
        Vector3 newPosition = Vector3.zero;
        Vector3 newScale = Vector3.zero;

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

            // Test this location with an overlap box that is high enough to catch the player in midair.
            // Also make it a little bit larger than the actual obstacle.
            Collider[] overlaps = Physics.OverlapBox(newPosition, new Vector3(newScale.x * 0.6f, 400, newScale.z * 0.6f));

            // Make sure this obstacle isn't going to be placed on top of the player or an enemy.
            placed = true;
            foreach (Collider collider in overlaps)
            {
                if (collider.tag == "Player" || collider.tag == "Enemy")
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
    }


    void PlaceEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab);
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
    }
}
