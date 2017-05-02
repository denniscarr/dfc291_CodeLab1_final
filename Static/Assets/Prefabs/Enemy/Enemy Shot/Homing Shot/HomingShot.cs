using UnityEngine;
using System.Collections;

public class HomingShot : EnemyShot {

    [SerializeField] private float minSpeed = 1f;
	[SerializeField] private float maxSpeed = 2f;  // The maximum speed at which I travel.
    [SerializeField] private float turnSpeed = 0.5f;  // The speed at which I accelerate towards my target.

    private Vector3 acceleration = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private Transform playerTransform;


	new void Start ()
    {
        base.Start();

        playerTransform = GameObject.Find("FPSController").transform;

        velocity = Vector3.Normalize(playerTransform.position - transform.position) * minSpeed;
	}
	

	new void Update ()
    {
        base.Update();

        acceleration = Vector3.Normalize(playerTransform.position - transform.position);
        acceleration *= (turnSpeed * Time.deltaTime);

        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        velocity.y = 0f;

        transform.position += velocity * Time.deltaTime;
	}
}
