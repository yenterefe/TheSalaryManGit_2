using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Material original;
    public Material tile;
    private AudioSource audio;
    public AudioClip scoreUpdate;
    private EnemyMovement enemyMovement;
    public GameObject [] enemy;
    public GameObject boss;
    public GameObject powerUp;
    public GameObject bubble;
    public GameObject player;
    public GameObject background1;
    public GameObject background2;
    public GameObject background3;
    public GameObject background4;
    public GameObject controller;
    public GameObject background5;
    public GameObject background6;
    public GameObject background7;
    public GameObject digitalClock;
    public GameObject carpet;
    public GameObject canvasScore;
    public GameObject CanvasHealth;
    public GameObject CanvasClock;
    public GameObject day;
    public GameObject plane;
    public GameObject newHighScore;
    private Vector3 spawnPos;
    private Vector3 spawnOffset = new Vector3(1, 1); 
    private Animator anim;
    public Button restartButton;
    public Button startGameButton;
    public Slider playerHealth;
    private Boss bossClass;
    private PlayerMovement playerMovement;
    public TextMeshProUGUI gameOver;
    public TextMeshProUGUI intro;
    public TextMeshProUGUI subIntro;
    public TextMeshProUGUI title;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI minClock;
    public TextMeshProUGUI hourClock;
    public TextMeshProUGUI aMPm;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI currentDay;
    public Vector3 finalPos = new Vector3(0,0, 0);  
    private float destroyIntroTime = 4f;
    private float hideText = 2f;
    private float initializeTime = 5f;
    private float respawnTime =10f;
    public float speedText = 0.5f;
    public int spawnCount = 0;
    private int enemyType;
    private int spawnNumber = 1;
    private int intMinClock;
    private int newMinClock;
    private int intHourClock = 9;
    private int newMinute;
    private int newHour;
    private int dayNumber =1;
    public bool playerIsDead = false;

    private void Awake()
    {

        // This will get the enemey movement script 
        enemyMovement = GetComponent<EnemyMovement>();

        // This will get the player's animator
        anim = player.GetComponent<Animator>();

        // This will get the player movement script 
        //playerMovement = GetComponent<PlayerMovement>(); 

        // initialize enemy type integer
        enemyType = Random.Range(0, 4);

        audio = Camera.main.GetComponent<AudioSource>();

    }

   

    // Start is called before the first frame update
    void Start()
    {
        // This will show the intro
        IntroText();
        
        // This will make the menu appear after 7 seconds
        StartCoroutine(PopMenu());

        // To find playerMovement since it is set inactive in the hierarchy 
        bossClass = boss.GetComponent<Boss>();

        highScore.text = "Maximum Days survived " + PlayerPrefs.GetInt("dayNumber");

        // Spawn power up 5 seconds after the game starts and repeat the respawn after every 7 secs
        InvokeRepeating("SpawnPowerUp", initializeTime, respawnTime);

  
    }
        

    // Update is called once per frame
    void Update()
    {

        QuitApp();

        highScore.text = "Maximum Days survived " + PlayerPrefs.GetInt("dayNumber");

        HighScore();

        dayText.text = "Day " + dayNumber;

        currentDay.text = "Current Day " + dayNumber;

        // initialize enemy type integer
        enemyType = Random.Range(0, enemy.Length);

        // Find how many enemies are on screen
        float numOfEnemiesOnScreen = FindObjectsOfType<EnemyMovement>().Length; 

         // if the number of enemies is 0, the game will spawn more enemies and one power up
          if (numOfEnemiesOnScreen ==0 && spawnCount !=3 && playerMovement.powerUp == false) 
          {
             playerMovement.powerUp = false;
             spawnCount++;
             spawnNumber = Random.Range(1, 5);
             Spawn(spawnNumber);
          }
         
         //Spawn boss
         else if (numOfEnemiesOnScreen == 0 && spawnCount ==3)
         {
            spawnCount = 0;
            spawnNumber = 0;
            playerMovement.powerUp = false;
            boss.gameObject.SetActive(true);
            
         }

        // Spawned power up will be destroyed if the player doesn't grab it
        Destroy(GameObject.Find("Power Up(Clone)"), 3);

        // Check on player life every frame and if player life is zero, "Game Over" will appear on the screen 
        GameOver();

        //if player reaches 6 o'clock, they win
        ScoreManager();
    }

    public void Spawn(int enemyWave)
    {
        // Spawn enemy will increase by own everytime the method is called 
        for (int i = 0; i < enemyWave; i++)
        {
            // Instantiate(spawnParticle, enemy.transform.position, enemy.transform.rotation);
            Instantiate(enemy[enemyType], SpawnPosition(), transform.rotation);
        }             
    }
    
    Vector3 SpawnPosition()
    {
        // Method that returns the coordinate that stays within the screen 
        //return transform.position = new Vector3(Random.Range(-(playerMovement.xValue), playerMovement.xValue), transform.position.y, Random.Range(-(playerMovement.bottomZValue), playerMovement.topZValue));
       spawnPos = new Vector3(Random.Range(-(playerMovement.xValue), playerMovement.xValue), transform.position.y, Random.Range(-(playerMovement.bottomZValue), playerMovement.topZValue));

        if (spawnPos != playerMovement.playerTransform)
        {
            return spawnPos;
        }

        else return spawnPos + spawnOffset;
    }

    public void SpawnPowerUp()
    {
        //Spawns powerup
        Instantiate(powerUp, SpawnPosition(), transform.rotation);
    }

    public void GameOver()
    {
        // If player life is zero the game will be over
        if (playerMovement.playerLife==0)          
        {
            // Set active the game over message
            gameOver.gameObject.SetActive(true);

            // player is dead bool will set as true so we can stop the boss animation to stop
            //playerIsDead = true;

            // Disable the bubble when player dies
            //bubble.gameObject.SetActive(false);

            // if player is dead it will play the death animation 
           // anim.SetBool("Death", true);

            // If player life is zero, player will be destroyed 
            //Destroy(playerMovement);

            // Destroy enemy
            //Destroy(enemy[enemyType]);

            // Destroy Boss
            //Destroy(boss);

            // Stop Invoke power up
            //CancelInvoke();
            Time.timeScale = 0f;

            // Activate restart button when player is dead
            restartButton.gameObject.SetActive(true);

            // Stop music 
            audio.enabled = false;
        }
    }

    public void StartGame()
    {
        // Scene gets reloaded when player clicks on the button
        // Since SceneManager was giving me issues on Android, I came up with this solution instead of loading the scene 
        restartButton.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        dayNumber = 1;
        intMinClock = 0;
        playerMovement.numOfEnemiesDestroyed = 0;
        intHourClock = 9;
        aMPm.text = "am";
        spawnCount = 0;
        spawnNumber = 1;
        audio.enabled = true;
        GameObject[] erase = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject destroy in erase)
        Destroy(destroy);
        boss.SetActive(false);
        bossClass.bossLife = 5;
        playerMovement.bossHealthBar.value = 1f;
        playerMovement.playerLife = 3f;
        playerMovement.playerHealthBar.value = 1f;
        Time.timeScale = 1f;
    }

    IEnumerator PopMenu()
    {
        // After the intro is destroyed the the menu will appear by start couroutining it in the start
        yield return new WaitForSeconds(destroyIntroTime);
        title.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(true);
    }
    

    public void LoadGame()
    {
        // Changing to wood tiles to make the environment cooler 
        plane.GetComponent<MeshRenderer>().material = tile;

        day.gameObject.SetActive(true);
        Invoke("DeactivateDay", 4);

        currentDay.gameObject.SetActive(true);

        canvasScore.gameObject.SetActive(true);

        digitalClock.gameObject.SetActive(true);

        playerHealth.gameObject.SetActive(true);

        carpet.gameObject.SetActive(true);

        CanvasHealth.gameObject.SetActive(true);

        CanvasClock.gameObject.SetActive(true);

        highScore.gameObject.SetActive(true);

        // To Set player as active
        player.gameObject.SetActive(true);

        // To find PlayerMovement script 
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

        // One enemy will be spawned at the beginning of the game
        Spawn(spawnNumber);
       
        // Deactivates title and the start button 
        title.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
        background1.SetActive(true);
        background2.SetActive(true);
        background3.SetActive(true);
        background4.SetActive(true);
        //controller.SetActive(true);
        background5.SetActive(true);
        background6.SetActive(true);
        background7.SetActive(true);


        // Play music 
        audio.enabled=true; // remove comment to play music later   
    }

    void IntroText()
    {
        // This method will destroy the quote and then the start game button and title will appear 
        Destroy(intro, destroyIntroTime);
        Destroy(subIntro, destroyIntroTime);
    }

    void DeactivateDay()
    {
        day.gameObject.SetActive(false);
    }

    void DeactiveScoreText()
    {
        newHighScore.gameObject.SetActive(false);
    }

    void NextRound()
    {
        // After player beats game, it will be the following day and the speed of enemies will increase by 0.5
        playerMovement.playerLife = 3f;
        playerMovement.playerHealthBar.value = 1f;
        dayNumber++;
        day.gameObject.SetActive(true);
        Invoke("DeactivateDay", 4f);
    }

    void ScoreManager()
    {
      Timer();
    }

    void Timer()
    {
        intMinClock = 00;
        // This line will replace the GUI with the variable 9 so the hour can be updated
        newHour = intHourClock;
        hourClock.text = newHour.ToString();

        // This line will update the minute and hour
        newMinute = intMinClock + (playerMovement.numOfEnemiesDestroyed * 10); // remove comment if it doesn't work
        minClock.text = ":" + newMinute.ToString(); // remove comment if it doesn't work 

        if (newMinute == 60)
        {
            // it will add plus one everytime it's a minute
            newHour = intHourClock++;
            hourClock.text = newHour.ToString();

            // And reset the minute to zero and the destroy counter will also be set to zero
            intMinClock = -60;
            playerMovement.numOfEnemiesDestroyed = 0;
            newMinute = intMinClock + (playerMovement.numOfEnemiesDestroyed * 10);
            minClock.text = ":" + newMinute.ToString();
        }

        if (newHour==12)
        {
            // when the clock hits noon, it will change from AM to PM
            aMPm.text = "PM";
        }

        if (newHour == 13)
        {
            // When hour equals to 1, it will set it to zero to show 1 pm since the methode increments by 1
            intHourClock = 0;
            newHour = intHourClock++;
            hourClock.text = newHour.ToString();
            intMinClock = -60;
            playerMovement.numOfEnemiesDestroyed = 0;
            newMinute = intMinClock + (playerMovement.numOfEnemiesDestroyed * 10);
            minClock.text = ":" + newMinute.ToString();
        }

        if (newHour == 5)
        {

            intMinClock = -60;
            playerMovement.numOfEnemiesDestroyed = 0;
            intHourClock = 9;
            aMPm.text = "AM";
            NextRound();
        }
    }

    void HighScore()
    {
        // This is to save players high score 
        // it will replace the saved value once player's dayNumber is bigger than the one that is saved 
        if (dayNumber > PlayerPrefs.GetInt("dayNumber"))
        {
            audio.PlayOneShot(scoreUpdate,0.25f);
            newHighScore.gameObject.SetActive(true);
            Invoke("DeactiveScoreText", hideText);
            PlayerPrefs.SetInt("dayNumber", dayNumber);
        }
        
        // To rest highscore so I can test it's functionality 
        /*if (Input.GetKey(KeyCode.Delete))
        {
            PlayerPrefs.DeleteKey("dayNumber");
        }*/
    }

    private void QuitApp()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
