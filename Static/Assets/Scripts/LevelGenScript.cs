using UnityEngine;
using System.Collections;

public class LevelGenScript : MonoBehaviour {

	public float levelSize;
	public float numberOfEnemies;
	public float numberOfObstacles;
	public float obstacleMinSize;
	public float obstacleMaxSize;

	public GameObject enemyPrefab;
	public GameObject obstaclePrefab;

	Transform playerSpawnPoint;
    Transform player;
    Transform floor;


	public void Awake()
    {
		playerSpawnPoint = GameObject.Find ("Player Spawn Point").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        floor = GameObject.Find("Floor").transform;
	}
		

	public void Generate ()
	{
		// Clear level
		GameObject[] stuffToDelete = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject go in stuffToDelete) {
			Destroy (go);
		}

		stuffToDelete = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach (GameObject go in stuffToDelete) {
			Destroy (go);
		}
			
		// Generate level
		for (int i = 0; i < numberOfObstacles; i++) {
			Instantiate (obstaclePrefab);
		}

		for (int i = 0; i < numberOfEnemies; i++) {
			Instantiate (enemyPrefab);
		}

		player.transform.position = new Vector3(player.transform.position.x, playerSpawnPoint.position.y, player.transform.position.z);
		floor.GetComponent<MeshCollider> ().enabled = true;
		GameObject.Find ("Game Manager").GetComponent<BatchBillboardScript> ().ReupdateBillboards ();
	}
}
