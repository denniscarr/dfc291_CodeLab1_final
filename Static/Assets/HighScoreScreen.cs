using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreScreen : MonoBehaviour {

	private void Update()
    {
        if (Input.GetAxis("Fire1") > 0.7f)
        {
            GameObject.Find("Game Manager").GetComponent<GameManager>().RestartGame();
        }
    }
}
