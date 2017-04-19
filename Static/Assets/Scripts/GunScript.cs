using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {

    /* TWEAKABLE VARIABLES */

    // How many bullets are fired per shot.
	[SerializeField] int bulletsPerBurstMin = 2;
	[SerializeField] int bulletsPerBurstMax = 30;

    // How many shots can be fired per second.
	[SerializeField] int burstsPerSecondMin = 1;
    [SerializeField] int burstsPerSecondMax = 5;

    // Bullet spread.
    [SerializeField] float inaccuracyMin = 1f;
    [SerializeField] float inaccuracyMax = 10f;

    // How far bullets can travel.
    [SerializeField] float bulletRange = 500f;

    // Bullet Color
    [SerializeField] Color bulletColor1;
    [SerializeField] Color bulletColor2;

    // How quickly the gun oscillates between extremes.
    [SerializeField] float oscSpeed = 0.3f;
    

    /* PREFABS */

	public GameObject bulletPrefab;
	public GameObject bulletStrikePrefab;

	public AudioSource shootAudio;
	public AudioSource bulletStrikeAudio;

    public GameObject muzzleFlash;


    /* REFERENCES */
    ScoreControllerScript scoreControllerScript;
    Transform bulletSpawnTransform; // The point where bullets originate (ie the tip of the player's gun
    Transform gunTipTransform;
    Animator animator;
    Animator parentAnimator;

    /* MISC */

    float timeSinceLastShot;
    GameObject[] bullets;    // Holds references to all bullets.
    int bulletIndex = 0;
    Color bulletColor = Color.yellow;  // The current color of the bullets.
    Vector3 originalPosition;   // The original position of the gun (used for recoil).
    Vector3 recoilPosition;

	void Start ()
    {
        // Instantiate all bullet prefabs. (Just make 100 for now so I don't have to do math to figure out how many could potentially be on screen at once.)
        // Then, move them all to a far away place so the player doesn't see them.
        bullets = new GameObject[100];
        for (int i = 0; i < 100; i++)
        {
            bullets[i] = Instantiate(bulletPrefab);
            bullets[i].transform.position = new Vector3(0, -500, 0);
        }

        // Get the point from which bullets will spawn.
		bulletSpawnTransform = GameObject.Find ("BulletSpawnPoint").transform;

        gunTipTransform = GameObject.Find("Tip").transform;

        // Get a reference to the score controller.
        scoreControllerScript = FindObjectOfType<ScoreControllerScript>();

        animator = GetComponent<Animator>();
        parentAnimator = GetComponentInParent<Animator>();

        originalPosition = transform.localPosition;
        recoilPosition = new Vector3(0f, -3.68f, 10.68f);
    }


	void Update ()
	{
		// Get new firing variables based on current oscillation.
		int bulletsPerBurst = Mathf.RoundToInt(MyMath.Map(Mathf.Sin(Time.time*oscSpeed), -1f, 1f, bulletsPerBurstMax, bulletsPerBurstMin));
		float burstsPerSecond = MyMath.Map (Mathf.Sin (Time.time*oscSpeed), -1f, 1f, burstsPerSecondMin, burstsPerSecondMax);
		float inaccuracy = MyMath.Map (Mathf.Sin (Time.time*oscSpeed), -1f, 1f, inaccuracyMax, inaccuracyMin);
		shootAudio.pitch = MyMath.Map (Mathf.Sin (Time.time * oscSpeed), -1f, 1f, 0.8f, 2f);

        // Update gun animation state
        animator.SetFloat("Gun State", Mathf.Sin(Time.time * oscSpeed));

        // Run shot timer.
        timeSinceLastShot += Time.deltaTime;

		if (Input.GetButton ("Fire1") && timeSinceLastShot >= 1/burstsPerSecond)
		{
			// Fire a burst
			FireBurst(bulletsPerBurst, inaccuracy);

            // Reset timer.
            timeSinceLastShot = 0f;
		}
	}

    
    // Firing a burst of bullets.
	void FireBurst(int numberOfBullets, float inaccuracy)
	{
        // Play shooting sound.
		shootAudio.Play ();

        transform.parent.SendMessage("IncreaseShake", 0.1f);

        // Show muzzle flash.
        GameObject _muzzleFlash = Instantiate(muzzleFlash);
        _muzzleFlash.transform.position = gunTipTransform.position;
        _muzzleFlash.transform.rotation = gunTipTransform.rotation;
        _muzzleFlash.transform.localScale = new Vector3(
            MyMath.Map(Mathf.Sin(Time.time * oscSpeed), -1f, 1f, 0.5f, 0.3f),
            MyMath.Map(Mathf.Sin(Time.time * oscSpeed), -1f, 1f, 0.5f, 0.3f),
            MyMath.Map(Mathf.Sin(Time.time * oscSpeed), -1f, 1f, 0.5f, 0.3f)
            );

        // Get a new bullet color based on current sine
        bulletColor = Color.Lerp(bulletColor1, bulletColor2, MyMath.Map(Mathf.Sin(Time.time * oscSpeed), -1f, 1f, 0f, 1f));

        // Fire the specified number of bullets.
		for (int i = 0; i < numberOfBullets; i++)
        {
			FireBullet (inaccuracy);
		}
	}


    // Firing an individual bullet.
	void FireBullet(float inaccuracy)
	{	
        // Rotate bullet spawner to get the direction of the next bullet.
		bulletSpawnTransform.localRotation = Quaternion.Euler(new Vector3(90+Random.insideUnitCircle.x * inaccuracy, 0, Random.insideUnitCircle.y * inaccuracy));

        // Declare variables for bullet size & position.
		float bulletScale;
		Vector3 bulletPosition;

        // Whether we should play the bullet hit sound.
        bool playAudio = false;

        // Raycast to see if the bullet hit an object and to see where it hit.
        RaycastHit hit;
        if (Physics.Raycast (bulletSpawnTransform.position, bulletSpawnTransform.up, out hit, bulletRange, 1 << 8)) {

			// Show particle effect
			Instantiate(bulletStrikePrefab, hit.point, Quaternion.identity);

			// The new bullet's y scale will be half of the ray's length
			bulletScale = hit.distance / 2;

			// The new bullet's position will be halfway down the ray
			bulletPosition = bulletSpawnTransform.up.normalized * (hit.distance / 2);

            // If the bullet hit an enemy...
			if (hit.collider.tag == "Enemy")
            {
                // Tell the enemy it was hurt.
                hit.collider.GetComponent<EnemyScript>().GetHurt();

                // Tell the score controller that the player hit an enemy with a bullet.
                scoreControllerScript.BulletHit();

                // We only want to play the bullet strike sound once, not once for every bullet that hit an enemy. So set a bool which tells the sound to play
                // later on.
				playAudio = true;
			}

		}

        // If the bullet did not strike anything give it a generic size and position.
        else
        {
			bulletScale = bulletRange/2;
			bulletPosition = bulletSpawnTransform.up.normalized*(bulletRange/2);
		}

        // If a bullet hit an enemy, play the bullet strike audio.
		if (playAudio)
        {
			bulletStrikeAudio.Play ();
		}

        // Set bullet color
        bullets[bulletIndex].GetComponent<MeshRenderer>().material.color = bulletColor;
        //bullets[bulletIndex].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", bulletColor);

        // Fire bullet.
        bullets[bulletIndex].GetComponent<PlayerBullet>().GetFired(
            bulletSpawnTransform.position + bulletPosition,
            bulletSpawnTransform.rotation,
            new Vector3(bulletPrefab.transform.localScale.x, bulletScale, bulletPrefab.transform.localScale.z)
        );

        // Get a new bullet index.
        bulletIndex += 1;
        if (bulletIndex >= 100)
        {
            bulletIndex = 0;
        }
    }
}
