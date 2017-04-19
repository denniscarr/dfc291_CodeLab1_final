using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public void PlayButton() {
        // Unpause everything and hide menu.
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<EnemyScript>().enabled = true;
        }
        GameObject.Find("FPSController").GetComponent<FirstPersonController>().enabled = true;
        GameObject.Find("Gun").GetComponent<GunScript>().enabled = true;

        transform.parent.gameObject.SetActive(false);
    }

	public void QuitButton() {
		Application.Quit ();
	}

	public void InfoButton() {
		SceneManager.LoadScene (1);
	}
}
