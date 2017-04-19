using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initial : MonoBehaviour {

    // Whether I am currently being controlled by the player.
    bool active;
    [HideInInspector] public bool Active
    {
        get { return active; }

        set
        {
            if (value == true)
            {
                textMesh.color = parentScript.activeColor;
            }

            else
            {
                textMesh.color = parentScript.inactiveColor;
            }
        }
    }

    [HideInInspector] public int charIndex;    // The character I am currently displaying.

    TextMesh textMesh;
    InitialEntry parentScript;

    void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        parentScript = GetComponentInParent<InitialEntry>();
    }

    public void SetChar(char newChar)
    {
        textMesh.text = newChar.ToString();
    }
}
