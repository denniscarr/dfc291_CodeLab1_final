using System.Collections;
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
    int currentEnemyAmt;    // The number of enemies currently alive in this level.
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

    // MISC REFERENCES
    Transform floor;    // The floor of the game environment.
    ScoreManager scoreManager;
    HealthManager healthManager;
    GameObject player;


    void Awake()
    {
        // Pause everything for the main menu.
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<Enemy>().enabled = false;
        }
        GameObject.Find("FPSController").GetComponent<FirstPersonController>().enabled = false;
        GameObject.Find("Gun").GetComponent<Gun>().enabled = false;
    }


    private void Start()
    {
        // Set up the current number of enemies.
        currentEnemyAmt = numberOfEnemies;

        // Get references
        floor = GameObject.Find("Floor").transform;
        scoreManager = GetComponent<ScoreManager>();
        healthManager = GetComponent<HealthManager>();
        levelGenerator = GetComponent<LevelGenerator>();
        player = GameObject.Find("FPSController");
    }


    private void Update()
    {
        // Run idle timer.
        if (gameStarted)
        {
            if (timeSinceLastInput >= idleResetTime)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            bool buttonPressed = false;
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick 1 button " + i.ToString()))
                {
                    buttonPressed = true;
                }
            }

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
        floor.GetComponent<MeshCollider>().enabled = false;

        // Increase level number.
        levelNumber += 1;

        // Set the number of enemies for the next level.
        numberOfEnemies = levelNumber * 7;

        // Generate a new level.
        levelGenerator.Invoke("Generate", 1.4f);
        currentEnemyAmt = numberOfEnemies;
    }


    public void GetHurt()
    {
        scoreManager.GetHurt();

        healthManager.playerHealth -= 1;

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
    }


    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }


    public void RestartGame()
    {
        SceneManager.LoadScene("mainScene");
    }


    
}
