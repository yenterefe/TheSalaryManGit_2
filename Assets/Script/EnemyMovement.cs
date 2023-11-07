using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Rigidbody enemyRB;
    private GameManager gameManager;
    public GameObject bubbleSkull;
    public GameObject bubbleSadFace;
    private Vector3 offSetBubble = new Vector3 (-1f,0f,1f);
    private PlayerMovement playerMovement;
    private float chasePlayerSpeed;
    private float chaseEnemySpeed;
    private float incrementalSpeed = 0.25f;
    public float minRandomNumber= 0.5f;
    public float maxRandomNumber = 0.9f;

    // Start is called before the first frame update

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();  

        // Get bubble offset from script
        playerMovement = GetComponent<PlayerMovement>();

        // Set random enemy speed
        chasePlayerSpeed = Random.Range(minRandomNumber,maxRandomNumber); 

        // set random chase speed
        chaseEnemySpeed = Random.Range(1, 6);

        // To find enemy's Rigidbody
        enemyRB = GetComponent<Rigidbody>();

        // To find Player GameObject with PlayerMovement script attached to it
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();       
    }

    // Update is called once per frame
    void Update()
    {
        // So enemy doesn't clip the ground
        transform.position = new Vector3(transform.position.x, 0.4f, transform.position.z);

        // If player has power up, enemy is going to runaway from player 
        if (playerMovement.powerUp== true) 
        {
            enemyRB.AddForce(-(playerMovement.transform.position).normalized * Time.deltaTime * chaseEnemySpeed);

            // Sad emoji will show when player has power up
            bubbleSkull.gameObject.SetActive(false);
            bubbleSadFace.gameObject.SetActive(true);
        }

        // If player does not have power up enemy will follow player    
         if (playerMovement.powerUp== false)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerMovement.transform.position, chasePlayerSpeed * Time.deltaTime);
            
            //  Skull emoji will show when player does not have power up
            bubbleSadFace.gameObject.SetActive(false);
            bubbleSkull.gameObject.SetActive(true);
        }

        // Bubble will always be on top of enemy
        bubbleSkull.transform.position = transform.position + offSetBubble;
        bubbleSadFace.transform.position = transform.position + offSetBubble;
       
    } 
}

    
