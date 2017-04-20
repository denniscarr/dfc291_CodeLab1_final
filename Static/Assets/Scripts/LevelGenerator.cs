using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    // LEVEL GEN VARIABLES
	public float levelSize; // The size of the level (the level is always square so this is the width and height.)
	public float numberOfEnemies;
	public float numberOfObstacles;
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
			Instantiate (obstaclePrefab);
		}

		for (int i = 0; i < numberOfEnemies; i++) {
			Instantiate (enemyPrefab);
		}

        // Place the player in the correct spot above the level.
		player.transform.position = new Vector3(player.transform.position.x, playerSpawnPoint.position.y, player.transform.position.z);

        // Re-enable the floor's collision (since it is disabled when the player completes a level.)
		floor.GetComponent<MeshCollider> ().enabled = true;

        // Update billboards.
		GameObject.Find ("Game Manager").GetComponent<BatchBillboard> ().UpdateBillboards ();
	}
}
