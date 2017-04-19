using UnityEngine;
using System.Collections;

public class CursorLockScript : MonoBehaviour {

	void Start () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}
}
