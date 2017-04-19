using UnityEngine;
using System.Collections;

public class PlacementScript : MonoBehaviour {

	LevelGenScript lgn;
	bool placed = false;

	void Start ()
	{
        placed = false;

		lgn = GameObject.Find ("Game Manager").GetComponent<LevelGenScript> ();

		Vector3 newPostion = transform.position;
		Vector3 newScale = transform.localScale;

		while (!placed)
		{
			if (gameObject.tag == "Obstacle")
            {	
				// Get my size
				newScale.x = Random.Range (lgn.obstacleMinSize, lgn.obstacleMaxSize);
				newScale.y = transform.localScale.y;
				newScale.z = Random.Range (lgn.obstacleMinSize, lgn.obstacleMaxSize);

				// Get my position
				newPostion.x = Random.Range (-lgn.levelSize + newScale.x / 2, lgn.levelSize - newScale.x / 2);
				newPostion.y = transform.localPosition.y;
				newPostion.z = Random.Range (-lgn.levelSize + newScale.z / 2, lgn.levelSize - newScale.z / 2);

				// Test this location with an overlap box that is high enough to catch the player in midair.
                // Also make it a little bit larger than the actual obstacle.
				Collider[] overlaps = Physics.OverlapBox (newPostion, new Vector3(newScale.x * 0.6f, 400, newScale.z * 0.6f));

				foreach (Collider c in overlaps)
                {
					if (c.tag == "Player" || c.tag == "Enemy") {
						print ("Not good");
						placed = false;
					} else {
						placed = true;
					}
				}
			}

			else if (gameObject.tag == "Enemy")
            {	
				// Get my position
				newPostion.x = Random.Range(-lgn.levelSize + GetComponent<Collider>().bounds.extents.x, lgn.levelSize - GetComponent<Collider>().bounds.extents.x);
				newPostion.y = transform.position.y;
				newPostion.z = Random.Range(-lgn.levelSize + GetComponent<Collider>().bounds.extents.z, lgn.levelSize - GetComponent<Collider>().bounds.extents.z);

                // Test this location
                Collider[] overlaps = Physics.OverlapSphere(newPostion, GetComponent<Collider>().bounds.extents.x * 1.5f);
				foreach (Collider c in overlaps)
                {
					if (c.tag == "Player" || c.tag == "Enemy" || c.tag == "Obstacle" || c.tag == "Wall") {
						print ("Not good" + c.tag);
						placed = false;
					} else {
                        print("Good Stuff! " + c.tag);
						placed = true;
					}
				}

                print(newPostion);
			}
		}

        //print("Made it");

		transform.position = newPostion;
		transform.localScale = newScale;

		enabled = false;
	}
}
