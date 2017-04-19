using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialEntry : MonoBehaviour {

    public Initial[] initials;
    Initial ActiveInitial
    {
        get
        {
            return initials[activeInitalIndex];
        }
    }
    char[] letters = { '_', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' }; 
    int activeInitalIndex = 0;   // The initial which is currently being controlled by the player.

    public Color activeColor;   // The color of the letter when it is active.
    public Color inactiveColor;

    float keyCooldown = 0.09f;   // How often a keypress is registered.
    float sinceLastKeypress = 0f;

    ScoreControllerScript scoreController;
    Transform gameOverScreen;
    Transform nameEntry;


    void Start()
    {
        scoreController = GameObject.Find("Score Display").GetComponent<ScoreControllerScript>();

        ActiveInitial.Active = true;
    }

	
	void Update ()
    {
        sinceLastKeypress += Time.deltaTime;

        /* PLAYER CONTROL */

        if (sinceLastKeypress >= keyCooldown && Input.anyKeyDown)
        {
            // If the player pressed up or down, change the character of the active initial.
            if (Input.GetAxisRaw("Vertical") == -1)
            {
                ActiveInitial.charIndex--;
                if (ActiveInitial.charIndex < 0) ActiveInitial.charIndex = letters.Length - 1;
                UpdateActiveInitialChar();
            }

            else if (Input.GetAxisRaw("Vertical") == 1)
            {
                ActiveInitial.charIndex++;
                if (ActiveInitial.charIndex > letters.Length - 1) ActiveInitial.charIndex = 0;
                UpdateActiveInitialChar();
            }

            // If the player pressed a vertical direction, switch active letter.
            else if (Input.GetAxisRaw("Horizontal") == -1)
            {
                ActiveInitial.Active = false;
                activeInitalIndex--;
                if (activeInitalIndex < 0) activeInitalIndex = initials.Length - 1;
                ActiveInitial.Active = true;
            }

            else if (Input.GetAxisRaw("Horizontal") == 1)
            {
                ActiveInitial.Active = false;
                activeInitalIndex++;
                if (activeInitalIndex > initials.Length - 1) activeInitalIndex = 0;
                ActiveInitial.Active = true;
            }

            // If the player is finished they should press fire.
            else if (Input.GetButtonDown("Fire1"))
            {
                // Go through each initial and add it to a string.
                string enteredInitials = "";
                bool cancel = false;
                foreach(Initial initial in initials)
                {
                    if (initial.charIndex != 0)
                    {
                        enteredInitials += letters[initial.charIndex].ToString();
                    }

                    // If the player hasn't had time to enter their initials then go back.
                    else
                    {
                        cancel = true;
                    }
                }

                // Tell the score controller to add this entry to its score list and then close this screen.
                if (!cancel)
                {
                    scoreController.InsertScore(enteredInitials);
                    
                }
            }

            sinceLastKeypress = 0f;
        }
	}


    void UpdateActiveInitialChar()
    {
        ActiveInitial.SetChar(letters[ActiveInitial.charIndex]);
    }
}
