using UnityEngine;
using System.Collections;

public class MouseReaderScript : MonoBehaviour {

	// Gets the position of the mouse relative to the size of the camera viewport

	public float x;
	public float y;

	int cameraWidth;
	int cameraHeight;

	void Start() {
		cameraWidth = Camera.main.pixelWidth;
		cameraHeight = Camera.main.pixelHeight;
	}

	void Update() {
		x = MyMath.Map (Input.mousePosition.x, 0f, cameraWidth, 0f, 1f);
		y = MyMath.Map (Input.mousePosition.y, 0f, cameraHeight, 0f, 1f);

		Debug.Log (x + ", " + y);
	}
}
