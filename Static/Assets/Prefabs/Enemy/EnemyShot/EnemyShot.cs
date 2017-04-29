using UnityEngine;
using System.Collections;

public class EnemyShot : MonoBehaviour {

	[SerializeField] private float speed = 2f;  // The speed at which I travel.
    private Vector3 direction;   // The direction in which I travel.
    private float maxLifetime = 30f;   // How long I live before I am deleted.
    private float currentLifetime;  // Used to track how long I have currently been alive.

    [SerializeField] private GameObject strikeParticles;    // The particles that spawn when I hit an enemy.

    GameManager gameManager;


	private void Start ()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // Get the direction towards the player.
        Vector3 targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y + 0.6f, targetPosition.z);
        direction = targetPosition - transform.position;
        direction.Normalize();
	}
	

	private void Update ()
    {
        // See if I have lived long enough and should be deleted.
        if (currentLifetime < maxLifetime)
        {
            currentLifetime += Time.deltaTime;
        }

        else
        {
            Destroy(gameObject);
        }

        // Move in my precalculated direction.
        transform.position = transform.position + direction * speed * Time.deltaTime;
	}


	void OnTriggerEnter(Collider collider)
    {
		if (collider.tag == "Obstacle" || collider.tag == "Wall" || collider.name == "Floor")
        {
            // Destroy self.
            Instantiate(strikeParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);	
		}

		else if (collider.tag == "Player")
        {
            gameManager.GetHurt();

            // Destroy self.
            Instantiate(strikeParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        } 
	}
}
