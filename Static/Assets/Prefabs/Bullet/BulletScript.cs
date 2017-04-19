using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    [SerializeField] float deleteTime = 0.25f;   // How long this bullet lasts on screen before being 'deleted'.
    float timer;    // How long this bullet has existed thus far.
    bool active;    // Whether this bullet is being fired.

	void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;
        }

		if (timer >= deleteTime)
        {
            // Move this bullet back to its holding location & make it tiny.
            transform.position = new Vector3(0, -500, 0);
            transform.localScale = new Vector3(1, 1, 1);

            active = false;
		}
	}

    public void GetFired(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        // Get proper transform values.
        transform.position = _position;
        transform.rotation = _rotation;
        transform.localScale = _scale;

        active = true;

        timer = 0f;
    }
}
