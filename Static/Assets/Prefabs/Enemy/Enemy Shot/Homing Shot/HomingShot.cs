using UnityEngine;
using System.Collections;

public class HomingShot : EnemyShot {

    // MOVEMENT
    [SerializeField] private float minSpeed = 1f;
	[SerializeField] private float maxSpeed = 2f;  // The maximum speed at which I travel.
    [SerializeField] private float turnSpeed = 0.5f;  // The speed at which I accelerate towards my target.

    private Vector3 acceleration = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 desiredVelocity = Vector3.zero;

    // VISUALS
    private Vector3 originalScale;
    [SerializeField] float scaleMin = 0.5f;
    [SerializeField] float scaleMax = 1.5f;

    [SerializeField] Color colorMin = Color.red;
    [SerializeField] Color colorMax = Color.yellow;

    private Transform playerTransform;


	new void Start ()
    {
        base.Start();

        playerTransform = GameObject.Find("FPSController").transform;

        velocity = Vector3.Normalize(playerTransform.position - transform.position) * minSpeed;
        desiredVelocity = velocity.normalized * maxSpeed;
	}
	

	new void Update ()
    {
        base.Update();

        /* MOVEMENT */

        acceleration = Vector3.zero;

        desiredVelocity = Vector3.Normalize(playerTransform.position - transform.position) * maxSpeed;
        Vector3 steerForce = Vector3.Normalize(desiredVelocity - velocity) * turnSpeed;

        acceleration += steerForce * Time.deltaTime;

        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        velocity.y = 0f;

        transform.position += velocity * Time.deltaTime;

        /* VISUALS */

        // Set scale based on velocity.
        float scaleScalar = MyMath.Map(velocity.magnitude, minSpeed, maxSpeed, scaleMin, scaleMax);
        Vector3 newScale = originalScale * scaleScalar;
        transform.localScale = newScale;

        // Set color based on velocity.
        //Color newColor = Color.Lerp(colorMin, colorMax, MyMath.Map(velocity.magnitude, minSpeed, maxSpeed, 0f, 1f));
        //GetComponent<MeshRenderer>().material.color = newColor;
	}
}
