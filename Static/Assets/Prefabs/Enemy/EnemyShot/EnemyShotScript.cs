using UnityEngine;
using System.Collections;

public class EnemyShotScript : MonoBehaviour {

	public float speed = 2f;

	Vector3 targetPosition;

    public GameObject strikeParticles;

	void Start () {
		
		// Get the player's current position and get a point way beyond that
		Vector3 directionToPlayer = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
		directionToPlayer.Normalize ();
		targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
		targetPosition += (directionToPlayer * 200);
		targetPosition.y = GameObject.FindGameObjectWithTag ("Player").transform.position.y;
	}
	
	void Update () {
		
		Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed*Time.deltaTime);
		transform.position = newPosition;

		if (transform.position == targetPosition) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider collider) {

		if (collider.tag == "Obstacle" || collider.tag == "Wall") {
			Debug.Log ("Hit obstacle");
            Instantiate(strikeParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);	
		}

		if (collider.tag == "Player") {

            Instantiate(strikeParticles, transform.position, Quaternion.identity);
			Destroy (gameObject);

			// Play various pain animations
			GameObject.Find ("Screen").BroadcastMessage ("GetHurt");
			GameObject.Find ("Screen").BroadcastMessage ("IncreaseShake", 0.3f);
			GameObject.Find ("Pain Flash").GetComponent<Animator> ().SetTrigger ("Pain Flash");

			GameObject.Find ("Score Display").SendMessage ("GetHurt");
		} 
	}
}
