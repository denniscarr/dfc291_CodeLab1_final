using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class MainMenuUI : MonoBehaviour {

	public void PlayButton()
    {
        // Unpause enemies in the background.
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<Enemy>().enabled = true;
        }

        // Enable player movement and shooting.
        GameObject.Find("FPSController").GetComponent<FirstPersonController>().enabled = true;
        GameObject.Find("Gun").GetComponent<GunScript>().enabled = true;

        // Turn off the main menu.
        transform.parent.gameObject.SetActive(false);
    }


	public void QuitButton()
    {
		Application.Quit ();
	}


	public void InfoButton()
    {
		SceneManager.LoadScene ("InfoScene");
	}
}
