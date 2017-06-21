using UnityEngine;
using System.Collections;

public class HomingShot : EnemyShot {

    enum HomingShotState { Homing, WasShot };
    HomingShotState state;

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

    // AUDIO
    [SerializeField] AudioSource blipAudioSource;

    // EXPLOSION
    [SerializeField] GameObject explosionPrefab;

    private Transform playerTransform;


	new void Start ()
    {
        base.Start();

        state = HomingShotState.Homing;

        gameManager.UpdateBillboards();
        playerTransform = GameObject.Find("FPSController").transform;

        //velocity = Vector3.Normalize(playerTransform.position - transform.position) * minSpeed;
        velocity = new Vector3(0f, 30f, 0f);
        desiredVelocity = velocity.normalized * maxSpeed;

        originalScale = transform.Find("Inner Sphere").localScale;
	}
	

	new void Update ()
    {
        base.Update();

        /* MOVEMENT */

        if (state == HomingShotState.Homing)
        {
            acceleration = Vector3.zero;

            desiredVelocity = Vector3.Normalize(playerTransform.position - transform.position) * maxSpeed;
            Vector3 steerForce = Vector3.Normalize(desiredVelocity - velocity) * turnSpeed;

            acceleration += steerForce * Time.deltaTime;

            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            //velocity.y = 0f;

            transform.position += velocity * Time.deltaTime;

            blipAudioSource.pitch = MyMath.Map(Vector3.Distance(playerTransform.position, transform.position), 15f, 60f, 1f, 0.3f);
        }


        /* VISUALS */

        // Set scale based on velocity.
        float scaleScalar = MyMath.Map(Vector3.Angle(velocity, desiredVelocity), 0f, 180f, scaleMax, scaleMin);
        Vector3 newScale = originalScale + (Random.insideUnitSphere * scaleScalar);
        transform.Find("Inner Sphere").localScale = newScale;
        //Debug.Log(newScale);

        // Set color based on velocity.
        //Color newColor = Color.Lerp(colorMin, colorMax, MyMath.Map(velocity.magnitude, minSpeed, maxSpeed, 0f, 1f));
        //GetComponent<MeshRenderer>().material.color = newColor;
    }


    public void GotShot(Vector3 forcePoint)
    {
        Debug.Log("Got shot");
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().AddExplosionForce(7f, forcePoint, 2f, 1f,  ForceMode.Impulse);
        blipAudioSource.pitch = 3f;
        Invoke("Detonate", 4f);
        state = HomingShotState.WasShot;
    }


    public override void Detonate()
    {
        base.Detonate();

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (state == HomingShotState.WasShot)
    //    {
    //        Detonate();
    //    }
    //}
}
