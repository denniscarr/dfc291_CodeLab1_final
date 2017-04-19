using UnityEngine;
using System.Collections;

public class ScreenShakeScript : MonoBehaviour {

	float shake = 0f;
	float shakeAmount = 0.3f;
	float decreaseFactor = 1.0f;
    float moveBackSpeed = 0.3f;

	Vector3 originalPosition;

	void Start() {
		originalPosition = transform.position;
	}

	void Update() {
		if (shake > 0f) {
			transform.position = originalPosition + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0) * shakeAmount;
//			camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, originalPosition.z);
			shake -= Time.deltaTime * decreaseFactor;

		} else {
			shake = 0.0f;

            // Move back towards original position.
            if (Vector3.Distance(transform.position, originalPosition) > 0.01f)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, originalPosition, moveBackSpeed * Time.deltaTime);
                transform.position = newPosition;
            }
        }
    }

	void IncreaseShake(float increaseAmount) {
		shake += increaseAmount;
	}
}
