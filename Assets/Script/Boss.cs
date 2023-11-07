using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private Rigidbody bossRB;
    private GameObject playerMovement;
    private GameManager gameManager;
    private AudioSource audioSource;
    public AudioClip stomping;
    public float bossStartAction = 0.5f;
    public float repeatAction = 4;
    public float bossLife = 5;
    public bool onTheGround;


    private void Awake()
    {
        // Getting rigid body to addforce to make boss jump
        bossRB = GetComponent<Rigidbody>();

        // Get the player gameobject to detemine the player's position 
        playerMovement = GameObject.Find("Player");

        //find GameManager object in the scene and find the game manager script
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Making the boss jump every 2 minutes 
        InvokeRepeating("BossJump", bossStartAction, repeatAction);
    }

    void BossJump()
    {
        // Methode to make the boss move and jump
        if (gameManager.playerIsDead == false) // if player is alive to keep jumping. If player is dead, the boss will stop jumping 
        {
            bossRB.AddForce(Vector3.up * 10, ForceMode.Impulse);
            transform.position = Vector3.MoveTowards(transform.position, playerMovement.transform.position, 2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Determines if boss is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            audioSource.PlayOneShot(stomping, 0.1f);
        }
    }
}

 


