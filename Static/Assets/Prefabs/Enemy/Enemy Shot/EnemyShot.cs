using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour {

    protected float maxLifetime = 30f;   // How long I live before I am deleted.
    protected float currentLifetime;  // Used to track how long I have currently been alive.

    [SerializeField] protected GameObject strikeParticles;    // The particles that spawn when I hit an enemy.

    protected GameManager gameManager;


    public void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }


    public void Update()
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
