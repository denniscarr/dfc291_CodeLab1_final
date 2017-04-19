using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuScript : MonoBehaviour {

	void Update() {
		if(Input.GetKeyDown("escape")) {
			MenuButton();
		}
	}

	public void MenuButton() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		SceneManager.LoadScene (0);
	}
}
