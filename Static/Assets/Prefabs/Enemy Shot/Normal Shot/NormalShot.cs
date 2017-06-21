using UnityEngine;
using System.Collections;

public class NormalShot : EnemyShot {

	[SerializeField] private float speed = 2f;  // The speed at which I travel.
    private Vector3 direction;   // The direction in which I travel.


	new void Start ()
    {
        base.Start();

        // Get the direction towards the player.
        Vector3 targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        direction = targetPosition - transform.position;
        direction.Normalize();
	}
	

	new void Update ()
    {
        base.Update();

        // Move in my precalculated direction.
        transform.position = transform.position + direction * speed * Time.deltaTime;
	}
}
