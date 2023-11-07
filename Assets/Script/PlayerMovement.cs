using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody playerRB;
    public GameObject boss;
    public GameObject bubble;
    public Slider playerHealthBar;
    public Slider bossHealthBar;
    public Slider coffeeBar;
    private Animator playerAnim;
    private Boss bossClass;
    private GameManager gameManager;
    private AudioSource playerAudioSource;
    public AudioClip explosion;
    public AudioClip powerUpClip;
    public AudioClip hitEnemySound;
    public AudioClip bossDestroy;
    public ParticleSystem playerHitParticle;
    public ParticleSystem enemyDestroyParticle;
    public ParticleSystem bossDestroyParticle;
    public ParticleSystem power;
    private Vector3 offsetBubble = new Vector3(1, 1, 1);
    public Vector3 playerTransform;
    private float horizontalMovement;
    private float verticalMovement;
    public float topZValue= 3f;
    public float bottomZValue = 4f; 
    public float xValue = 7f;
    public float playerSpeed = 10f;
    public float bossHit;
    public float bossLife = 5f;
    public float clockSpeed = 10f;
    public float yAxisHand = 10f; 
    public int playerScore = 0;
    public int numOfEnemiesDestroyed = 0;
    public float playerLife = 3f;
    public float playerMaximumLife=3f;
    public bool powerUp = false; 
    public bool hitEnemy= false;
    private float time = 5;
    private float currentCoffeeDuration=5;
    private float maximumCoffeeDuration = 5;

    private void Awake()
    {
        // Get game manager script
        gameManager = GetComponent<GameManager>();

        // Get player animator component
        playerAnim = GetComponent<Animator>();

        // Getting component of rigid body
        playerRB = GetComponent<Rigidbody>();

        // Find the boss script in the gameobject boss and store it in the bossClass variable 
        bossClass = boss.GetComponent<Boss>();

        // Initializing player audio source
        playerAudioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //To initiate the countdown twhen player collides with the power up
        StartCoroutine(PowerUpCountDown());        
    }

    // Update is called once per frame
    void Update()
    {

        playerTransform = transform.position;

        // Getting player vertical player input by arrow or keyboard
        horizontalMovement = SimpleInput.GetAxis("Horizontal") * Time.deltaTime;

        // Triggers animation based on the horizontal axis value 
        playerAnim.SetFloat("Horizontal", horizontalMovement);

        // Getting player vertical player input by arrow or keyboard
        verticalMovement = SimpleInput.GetAxis("Vertical") * Time.deltaTime;

        // Triggers animation based on the vertical axis value
        playerAnim.SetFloat("Vertical", verticalMovement);       

        // Move player when buttons are pressed 
        transform.Translate(horizontalMovement * playerSpeed, transform.position.y, verticalMovement * playerSpeed); // Might have to keep it 
              
        // Bubble will always be on top of player 
        bubble.transform.position = transform.position + offsetBubble;
       
        // To count in seconds how many time is left on the power up
        if (powerUp==true)
        {
            coffeeBar.gameObject.SetActive(true);
            time -= Time.deltaTime;
            currentCoffeeDuration = (int) time;
            UpdateCoffeeMeter(currentCoffeeDuration, maximumCoffeeDuration);           
        }

        else if ( powerUp==false)
        {
            coffeeBar.gameObject.SetActive(false);
            time = 5;
        }
    }

    void LateUpdate()
    {
        // Left side Screen limit so player does not go outisde of camera view
        if (transform.position.x < -xValue)
        {
            transform.position = new Vector3(-xValue, transform.position.y, transform.position.z);
        }

        // Right side Screen limit so player does not go outisde of camera view
        else if (transform.position.x > xValue)
        {
            transform.position = new Vector3(xValue, transform.position.y, transform.position.z);
        }

        // Top screen limit so player does not go outside camera view
        if (transform.position.z > topZValue)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y,topZValue);
        }

        // Bottom screen limit so player does not go outside camera view
        if (transform.position.z < -bottomZValue)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -bottomZValue);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // To check if the player collided with any GameObject 
        if (other.gameObject.CompareTag("PowerUP"))
        {
            playerAudioSource.PlayOneShot(powerUpClip, 0.1f);

            power.Play();

            // If player collides with power up it will be set to true
            powerUp = true;
    
            // Set bubble as disactive
            bubble.gameObject.SetActive(false);

            // The power up will be destroyed
            Destroy(other.gameObject);
            

            // This will turn off the power up after 3 secs
            StartCoroutine(PowerUpCountDown());
        }

        if (other.gameObject.CompareTag("Enemy") && powerUp==true)
        {
            playerAudioSource.PlayOneShot(hitEnemySound, 0.1f);

            // If player collides with enemy and has power up, enemy gets destroyed
            DestroyEnemy(other);

            enemyDestroyParticle.Play();

            // This will turn off the power up after 3 secs
            StartCoroutine(PowerUpCountDown());
        }

        if (other.gameObject.CompareTag("Enemy") && powerUp == false)
        {
            // It will detect if the player got hit by an enemy 
            hitEnemy = true;

            DestroyEnemy(other); 
            playerAudioSource.PlayOneShot(explosion, 0.1f);

            // A particle effect will happen when the player is hit
            playerHitParticle.Play();

            // Everytime a player gets hit, it will substract one life. If the player has zero life the game will end
            playerLife--;
            UpdatePlayerhealthBar(playerLife, playerMaximumLife);
        }

        if (other.gameObject.CompareTag("Boss") && powerUp == false)
        {
            // It will detect if the player got hit by an enemy 
            hitEnemy = true;

            playerAudioSource.PlayOneShot(explosion, 0.1f);

            // A particle effect will happen when the player is hit
            playerHitParticle.Play();

            // Everytime a player gets hit, it will substract one life. If the player has zero life the game will end
            playerLife--;
            UpdatePlayerhealthBar(playerLife, playerMaximumLife);
        }

       if (other.gameObject.CompareTag("Boss") && powerUp == true)
        {
            playerAudioSource.PlayOneShot(hitEnemySound, 0.1f);


            // Boss go hit by a player
            hitEnemy = false;
            bossClass.bossLife--;
            UpdateBosshealthBar(bossClass.bossLife, bossLife);
            StartCoroutine(PowerUpCountDown());

            // If boss life is zero, deactivate boss
            if (bossClass.bossLife ==0)
            {
                playerAudioSource.PlayOneShot(bossDestroy, 0.1f);

                bossDestroyParticle.Play();

                // deactovate  boss and set his health back to 100
                boss.gameObject.SetActive(false);

                // Reset the healthbar
                bossHealthBar.value = 1f;

                // Reset boss life
                bossClass.bossLife = 5f;

                // Set spawn count to zero to make the game fair 
                gameManager.spawnCount = 0;
            }
        }
    }

    private IEnumerator PowerUpCountDown()
    {
        // Counts how long player has opwered up and will deactivate in 5 seconds
        yield return new WaitForSeconds(5);

        // Set bubble as active
        bubble.gameObject.SetActive(true);

        // Set power up bool as false
        powerUp = false;
    }

    // Method to update boss healthbar 
    void UpdatePlayerhealthBar(float currentHealth, float maximumHealth)
    {
        playerHealthBar.value = currentHealth / maximumHealth;
    }

    // Method to update boss healthbar 
    void UpdateBosshealthBar(float currentHealth, float maximumHealth)
    {
       bossHealthBar.value = currentHealth / maximumHealth;
    }

    // Method to update boss healthbar 
    void UpdateCoffeeMeter(float currentTimer, float maximumCoffeeDuration)
    {
        coffeeBar.value = currentCoffeeDuration / maximumCoffeeDuration;
    }

    void DestroyEnemy(Collider other)
    {
        // This will count how many times an enemy was destroyed with a powerup       
        Destroy(other.gameObject);

        if (powerUp==true)
        {
            numOfEnemiesDestroyed = numOfEnemiesDestroyed + 3;
        }
    }
}

