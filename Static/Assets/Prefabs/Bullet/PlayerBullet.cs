using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    float deleteTime = 0.25f;   // How long this bullet lasts on screen before being 'deleted'.
    private float timeOnScreen;    // How long this bullet has existed thus far.
    private bool isOnScreen;    // Whether this bullet is being fired.


    private void Update()
    {
        // If this bullet is currently onscreen, track how long it has been on screen for.
        if (isOnScreen)
        {
            timeOnScreen += Time.deltaTime;
        }

        // If this bullet has existed long enough to be deleted then delete it.
        if (timeOnScreen >= deleteTime)
        {
            // Move this bullet back to its holding location & make it tiny.
            transform.position = new Vector3(0, -500, 0);
            transform.localScale = new Vector3(1, 1, 1);

            isOnScreen = false;
        }
    }


    /// <summary>
    /// Moves the bullet to the given position, rotation and scale, then sets it as active.
    /// </summary>
    /// <param name="_position">The position of the fired bullet.</param>
    /// <param name="_rotation">The rotation of the fired bullet.</param>
    /// <param name="_scale">The scale of the fired bullet</param>
    public void GetFired(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        // Get proper transform values.
        transform.position = _position;
        transform.rotation = _rotation;
        transform.localScale = _scale;

        // Set this bullet to active.
        isOnScreen = true;

        // Reset this bullet's timer.
        timeOnScreen = 0f;
    }
}