using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    // USED FOR DISPLAYING THE SCORE
    private int _score = 0;
    public int score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = Mathf.RoundToInt(score + value * multiplier);
            scoreDisplay.text = _score.ToString();
        }
    }// The player's current score. 
    [SerializeField] private TextMesh scoreDisplay;   // A reference to the TextMesh which displays the score.

    // USED FOR THE MULTIPLIER BAR
    [SerializeField] private GameObject multiplierBar;    // A reference to the multiplier bar GameObject.
    [SerializeField] private float multBarStartVal = 0.4f;    // How large of a multiplier the player starts the game with.
    [SerializeField] private float multBarBaseDecay = 0.01f;  // How quickly the multiplier bar shrinks (increases as the player's multiplier increases)
    [SerializeField] private float multBarSizeMin = 0.03f;    // The multiplier bar's smallest size.
    [SerializeField] private float multBarSizeMax = 7.15f;    // The multiplier bar's largets size.
    float multBarValueCurr;   // The current size of the multiplier bar (as percentage of it's max size).
    float multBarDecayCurr;     // How quickly the multiplier bar currently shrinks.
    float multBarStartValCurr;  // Where the multiplier bar starts at the player's current multiplier (decreases as the multiplier increases).

    [SerializeField] private TextMesh multNumber; // The TextMesh which displays the player's current multiplier.
    float multiplier = 1f;  // The multiplier that the player starts the game with.

    // HIGH SCORE LIST
    [SerializeField] public List<ScoreEntry> highScoreList;
    [SerializeField] Transform highScoreListText;
    [SerializeField] Transform highScoreScreen;

    // SCORE/MULTIPLIER VALUE OF VARIOUS THINGS
    [SerializeField] private int enemyScoreValue = 1000;  // How many points the player gets for killing an enemy (without multiplier applied)
    [SerializeField] private int levelWinScoreValue = 3000;
    [SerializeField] private float enemyMultValue = 0.5f; // How much the player's multiplier increases for killing an enemy.
    [SerializeField] private float bulletHitValue = 0.01f;    // How much the player's multiplier increases for hitting an enemy with one bullet.
    [SerializeField] private float getHurtPenalty = 0.1f; // How much the multiplier bar decreases when the player is hurt.

    // AUDIO
    [SerializeField] private AudioSource levelWinAudio;   // The audio source that plays when the player completes a level.
    [SerializeField] private AudioSource playerHurtAudio; // The audio source that plays when the player is hurt.

    // USED FOR LEVEL GENERATION
    public int levelNumber = 0;    // The current level.
    int numberOfEnemies = 4;    // The number of enemies that spawned in the current level.
    int currentEnemyAmt;    // The number of enemies currently alive in this level.
    LevelGenerator levelGenerator;  // A reference to the level generator script.

    // MISC REFERENCES
    Transform floor;    // The floor of the game environment.
    [SerializeField] Transform gameOverScreen;  // The game over screen.
    [SerializeField] Transform nameEntry;   // The name entry screen.
    [SerializeField] GameObject mainMenu;   // The main menu screen.

    void Awake()
    {
        // Pause everything for the main menu.
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<Enemy>().enabled = false;
        }
        GameObject.Find("FPSController").GetComponent<FirstPersonController>().enabled = false;
        GameObject.Find("Gun").GetComponent<GunScript>().enabled = false;
    }


    void Start()
    {
        // Set up the multiplier bar.
        multBarValueCurr = multBarStartVal;
        multBarDecayCurr = multBarBaseDecay;
        multBarStartValCurr = multBarStartVal;
        multiplierBar.transform.localScale = new Vector3(
            multiplierBar.transform.localScale.x,
            MyMath.Map(multBarValueCurr, 0f, 1f, multBarSizeMin, multBarSizeMax),
            multiplierBar.transform.localScale.z
        );

        // Set up the score and multiplier number displays.
        scoreDisplay.text = score.ToString();
        multNumber.text = multiplier.ToString() + "X";

        // Set up the current number of enemies.
        currentEnemyAmt = numberOfEnemies;

        // Get references to floor and level generator.
        floor = GameObject.Find("Floor").transform;
        levelGenerator = GetComponent<LevelGenerator>();

        // Set up high score list.
        highScoreList = LoadHighScores();
    }


    void Update()
    {
        // If the player presses fire on the high scores screen, reload the main scene.
        if (highScoreScreen.gameObject.activeSelf)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SceneManager.LoadScene("mainScene");
            }
        }

        // Apply decay to the multiplier bar
        multBarValueCurr -= multBarDecayCurr * Time.deltaTime;
        multBarValueCurr = Mathf.Clamp(multBarValueCurr, 0f, 1f);

        // If the multiplier bar value has gone below zero, lower the player's multiplier.
        if (multBarValueCurr <= 0f && multiplier > 1f)
        {
            // Lower the multiplier.
            multiplier -= 0.1f;

            // Get the values for the new multiplier level.
            multBarStartValCurr = multBarStartVal / multiplier;
            multBarValueCurr = multBarStartValCurr;
            multBarDecayCurr = multBarBaseDecay * multiplier;
            multNumber.text = multiplier.ToString() + "X";
        }

        // Update the size of the multiplier bar.
        multiplierBar.transform.localScale = new Vector3(
            multiplierBar.transform.localScale.x,
            MyMath.Map(multBarValueCurr, 0f, 1f, multBarSizeMin, multBarSizeMax),
            multiplierBar.transform.localScale.z
        );
    }


    /// <summary>
    /// Should be called when the player kills an enemy.
    /// </summary>
    public void KilledEnemy()
    {
        // Increase the multiplier.
        float multBarIncreaseAmt = enemyMultValue / multiplier;

        // Increase the value of the multiplier bar.
        multBarValueCurr += multBarIncreaseAmt;

        // If value of the multiplier bar has gotten to 1, raise the player's multiplier and set the multiplier bar values for the new multiplier level.
        if (multBarValueCurr >= 1f)
        {
            multiplier += 0.1f;
            multBarStartValCurr = multBarStartVal / multiplier;
            multBarValueCurr = multBarStartValCurr;
            multBarDecayCurr = multBarBaseDecay * multiplier;
        }

        // Update the multiplier number display.
        multNumber.text = multiplier.ToString() + "X";

        // Round the score to an integer and update the score display.
        score += enemyScoreValue;

        // See if the player has killed all the enemies in this level. If so, change the level.
        currentEnemyAmt -= 1;
        if (currentEnemyAmt <= 0)
        {
            EndLevel();
        }
    }


    /// <summary>
    /// Is called when player beats the current level.
    /// </summary>
    void EndLevel()
    {
        levelWinAudio.Play();

        // Disable the floor's collider so the player falls through it.
        floor.GetComponent<MeshCollider>().enabled = false;

        // Increase level number.
        levelNumber += 1;

        // Set the number of enemies for the next level.
        numberOfEnemies = levelNumber * 7;

        // Give the player a score boost for beating the level.
        score += levelWinScoreValue;

        // Generate a new level.
        levelGenerator.Invoke("Generate", 1.4f);
        currentEnemyAmt = numberOfEnemies;
    }
    

    /// <summary>
    /// Should be called when a bullet hits an enemy.
    /// </summary>
    public void BulletHit()
    {
        score += Mathf.RoundToInt(multiplier);
        multBarValueCurr += bulletHitValue;
    }


    /// <summary>
    /// Should be called when the player gets hurt.
    /// </summary>
    void GetHurt()
    {  
        playerHurtAudio.Play();
        multBarValueCurr -= getHurtPenalty;
    }


    /// <summary>
    /// Loads saved high scores from PlayerPrefs.
    /// </summary>
    /// <returns>List<ScoreEntry></returns>
    List<ScoreEntry> LoadHighScores()
    {
        // Declare a new high score list.
        List<ScoreEntry> newHighScoreList = new List<ScoreEntry>();

        // Initialize 10 empty score entries in the highScore List.
        for (int i = 0; i < 10; i++)
        {
            newHighScoreList.Add(new ScoreEntry("AAA", 0));
        }

        // Load saved high scores from PlayerPrefs.
        for (int i = 0; i < 10; i++)
        {
            // Check to see if this score entry has previously been saved.
            if (PlayerPrefs.GetString("HighScoreName" + i) != "")
            {
                newHighScoreList[i] = new ScoreEntry(PlayerPrefs.GetString("HighScoreName" + i), PlayerPrefs.GetInt("HighScoreNumber" + i));
            }
        }

        return newHighScoreList;
    }


    // Saves high scores
    void SaveHighScores()
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetString("HighScoreName" + i, highScoreList[i].initials);
            PlayerPrefs.SetInt("HighScoreNumber" + i, highScoreList[i].score);
        }
    }


    // Shows the high score list.
    public void ShowHighScores()
    {
        highScoreList = LoadHighScores();

        highScoreScreen.gameObject.SetActive(true);

        string scoreList = "";

        for (int i = 0; i < highScoreList.Count; i++)
        {
            scoreList += highScoreList[i].initials + ": " + highScoreList[i].score + "\n";
        }

        highScoreListText.GetComponent<TextMesh>().text = scoreList;
    }


    // Insters a score into the list.
    public void InsertScore(string initials)
    {
        LoadHighScores();

        //bool inserted = false;
        //for (int i = 0; i < highScoreList.Count; i++)
        //{
        //    Debug.Log(highScoreList[i]);

        //    // See if the current score is greater than this score on the list.
        //    if (score > highScoreList[i].score)
        //    {
        //        if (!inserted)
        //        {
        //            highScoreList.Insert(i, new ScoreEntry(initials, score));
        //            inserted = true;
        //        }
        //    }

        //    else
        //    {
        //        highScoreList.RemoveAt(i);
        //    }
        //}

        // Add and sort list
        highScoreList.Add(new ScoreEntry(initials, score));
        highScoreList.Sort(delegate (ScoreEntry b, ScoreEntry a)
        {
            return (a.score.CompareTo(b.score));
        });

        // Remove excess entries
        for (int i = 10; i < highScoreList.Count; i++)
        {
            highScoreList.RemoveAt(i);
        }

        // Stop showing game over screen and show high score list instead.
        gameOverScreen.gameObject.SetActive(false);
        nameEntry.gameObject.SetActive(false);
        SaveHighScores();
        ShowHighScores();
    }


    public class ScoreEntry
    {
        public string initials;
        public int score;

        public ScoreEntry(string _initials, int _score)
        {
            initials = _initials;
            score = _score;
        }
    }
}
