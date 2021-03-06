﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour {

    // AUDIO
    [SerializeField] private AudioSource levelWinAudio;   // The audio source that plays when the player completes a level.

    // USED FOR LEVEL GENERATION
    public int levelNumber = 0;    // The current level.
    int numberOfEnemies = 4;    // The number of enemies that spawned in the current level.
    public int currentEnemyAmt;    // The number of enemies currently alive in this level.
    LevelGenerator levelGenerator;  // A reference to the level generator script.

    // MENU SCREENS
    [SerializeField] GameObject highScoreScreen;
    [SerializeField] GameObject gameOverScreen; 
    [SerializeField] GameObject nameEntryScreen;
    [SerializeField] GameObject mainMenuScreen; 

    // USED FOR TIMER
    [SerializeField] float idleResetTime = 20f;
    float timeSinceLastInput = 0f;
    public bool gameStarted = false;

    // USED FOR SINE TRACKER
    public float currentSine;
    public float oscSpeed = 0.3f;
    [SerializeField] float bulletHitSineIncrease = 0.01f;
    float sineTime = 0.0f;

    // MISC REFERENCES
    Transform floor;    // The floor of the game environment.
    ScoreManager scoreManager;
    HealthManager healthManager;
    [HideInInspector] public GameObject player;


    void Awake()
    {
        // Pause everything for the main menu.
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<Enemy>().enabled = false;
        }
        GameObject.Find("FPSController").GetComponent<FirstPersonController>().enabled = false;
        foreach(Gun gun in FindObjectsOfType<Gun>())
        {
            gun.enabled = false;
        }
    }


    private void Start()
    {
        // Set up the current number of enemies.
        currentEnemyAmt = numberOfEnemies;

        //currentSine = Mathf.Sin(Time.time * oscSpeed);
        currentSine = Mathf.Sin(sineTime);

        // Get references
        floor = GameObject.Find("Floor").transform;
        scoreManager = GetComponent<ScoreManager>();
        healthManager = GetComponent<HealthManager>();
        levelGenerator = GetComponent<LevelGenerator>();
        player = GameObject.Find("FPSController");
    }


    private void Update()
    {
        // Update sine
        //currentSine = Mathf.Sin(sineTime * oscSpeed);
        currentSine = Mathf.Sin(sineTime);
        //currentSine = MyMath.Map(player.transform.rotation.eulerAngles.y, 0f, 360f, -1f, 1);

        // Run idle timer.
        if (gameStarted)
        {
            if (timeSinceLastInput >= idleResetTime)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // Check all joystick buttons.
            bool buttonPressed = false;
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick 1 button " + i.ToString()))
                {
                    buttonPressed = true;
                }
            }

            // See if any other buttons or keys have been pressed.
            if (Input.anyKeyDown || buttonPressed || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                timeSinceLastInput = 0f;
            }

            timeSinceLastInput += Time.deltaTime;
        }
    }


    public void KilledEnemy()
    {
        // See if the player has killed all the enemies in this level. If so, change the level.
        scoreManager.KilledEnemy();
        currentEnemyAmt -= 1;

        if (currentEnemyAmt <= 0)
        {
            LevelBeaten();
        }
    }


    public void LevelBeaten()
    {
        levelWinAudio.Play();

        scoreManager.LevelBeaten();

        // Disable the floor's collider so the player falls through it.
        floor.GetComponent<Collider>().enabled = false;

        // Increase level number.
        levelNumber += 1;

        // Generate a new level.
        levelGenerator.Invoke("Generate", 1.4f);
    }


    public void PlayerHurt()
    {
        scoreManager.GetHurt();

        healthManager.playerHealth -= 5;

        // If health is now less than zero, trigger a game over.
        if (healthManager.playerHealth <= 0)
        {
            ShowGameOverScreen();
        }
    }


    public void ShowHighScores()
    {
        scoreManager.LoadScoresForHighScoreScreen();

        gameOverScreen.gameObject.SetActive(false);
        nameEntryScreen.gameObject.SetActive(false);
        highScoreScreen.gameObject.SetActive(true);
    }


    public void BulletHit()
    {
        scoreManager.BulletHit();
        sineTime += bulletHitSineIncrease;
    }


    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }


    public void RestartGame()
    {
        SceneManager.LoadScene("mainScene");
    }


    public void UpdateBillboards()
    {
        FindObjectOfType<BatchBillboard>().UpdateBillboards();
    }
}
