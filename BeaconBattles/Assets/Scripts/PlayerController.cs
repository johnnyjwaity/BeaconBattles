using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class PlayerController : MonoBehaviour {

    public float walkSpeed;
    public float runSpeed;
    public float jumpStrength;
    public int maxHealth;
    private int currentHealth;
    public GameObject[] spawnPoints;
    private GameObject spawn;
    public float yMin;
    public float rotateSensitivity;
    private Rigidbody rb;
    private Animator anim;

    private bool canJump = true;

    private Vector3 originalRotation;

    private Multiplayer multi;
    public Attack[] attacks;
    private bool knockedBack;
    private float knockbackCounter = 0;
    public Slider healthBar;
    private bool isDead;
    public float respawnTime;
    private float respawnCounter;
    public GameObject respawnScreen;
    public TextMeshProUGUI respawnText;
    private int color;
    private BeaconManager beaconManager;
    public GameObject winScreen;
    public TextMeshProUGUI winText;
    private bool isReturning = false;
    private float returnCounter;
    public TextMeshProUGUI returnText;
    private bool won = false;

    // Use this for initialization
    void Start () {
        currentHealth = maxHealth;
        healthBar.value = 1;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        originalRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        multi = FindObjectOfType<Multiplayer>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        FindObjectOfType<Multiplayer>().syncQueue.Add(gameObject);
        color = PlayerPrefs.GetInt("color");
        Debug.Log("Color: " + color);
        spawn = spawnPoints[PlayerPrefs.GetInt("color")];
        transform.position = spawn.transform.position;
        beaconManager = FindObjectOfType<BeaconManager>();
	}
    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    // Update is called once per frame
    void Update () {
        if (knockedBack)
        {
            knockbackCounter -= Time.deltaTime;
            if(knockbackCounter <= 0){
                knockedBack = false;
            }
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        string movementType = "walk";
        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementType = "run";
            speed = runSpeed;
        }

        Vector3 forward = transform.forward * vertical;
        Vector3 right = transform.right * horizontal;

        Vector3 newVelocity = new Vector3(forward.x + right.x, 0, forward.z + right.z);
        newVelocity.Normalize();

        if (!knockedBack)
        {
            rb.velocity = new Vector3(newVelocity.x * speed, rb.velocity.y, newVelocity.z * speed);
        }
        

        

        if (horizontal != 0 || vertical != 0)
        {
            if(anim.GetBool(movementType) != true)
            {
                anim.SetBool(movementType, true);
                sendAnimation(movementType, "true");
            }
            if(anim.GetBool((movementType == "walk") ? "run" : "walk"))
            {
                sendAnimation((movementType == "walk") ? "run" : "walk", "false");
                anim.SetBool((movementType == "walk") ? "run" : "walk", false);
            }
        }
        else
        {
            if (anim.GetBool("run"))
            {
                sendAnimation("run", "false");
                anim.SetBool("run", false);
                
            }
            if(anim.GetBool("walk"))
            {
                sendAnimation("walk", "false");
                anim.SetBool("walk", false);
            }
        }
        

        float mouseX = Input.GetAxis("Mouse X");
        originalRotation.y += mouseX * rotateSensitivity;

        transform.rotation = Quaternion.Euler(new Vector3(originalRotation.x, originalRotation.y, originalRotation.z));

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {

            rb.AddForce(Vector3.up * jumpStrength);
            canJump = false;
        }

        if(transform.position.y < yMin)
        {
            Die();
        }
        if(currentHealth <= 0)
        {
            Die();
        }
        if (isDead)
        {
            respawnCounter -= Time.deltaTime;
            respawnText.text = "Respawn in " + Mathf.Round(respawnCounter);
            if (float.IsInfinity(respawnCounter))
            {
                respawnText.text = "You Will Not Respawn";
            }
            if(respawnCounter <= 0)
            {
                isDead = false;
                Respawn();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            sendAnimation("attack", "trigger");
            anim.SetTrigger("attack");
            attacks[0].run();
        }
        if (Input.GetMouseButtonDown(1))
        {
            sendAnimation("stab", "trigger");
            anim.SetTrigger("stab");
            attacks[1].run();
        }
        int winner = beaconManager.GetWinningBeacon();
        if(winner != -1)
        {
            GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("Enemy");
            int amountSurviving = 0;
            foreach(GameObject g in otherPlayers)
            {
                if(g.transform.position.y > -400)
                {
                    amountSurviving++;
                }
            }
            if(transform.position.y > -400)
            {
                amountSurviving++;
            }
            if (amountSurviving <= 1)
            {
                string text = "You Lost";
                if (winner == color)
                {
                    text = "You Won";
                    won = true;
                }
                if(gameObject.transform.position.y > -400)
                {
                    text = "You Won";
                    won = true;
                }
                winText.text = text;
                winScreen.SetActive(true);
                if (!isReturning)
                {
                    isReturning = true;
                    returnCounter = 3;
                }
            }
        }
        if (isReturning)
        {
            returnCounter -= Time.deltaTime;
            returnText.text = "Returning To Lobby in " + Mathf.Round(returnCounter);
            if(returnCounter <= 0)
            {
                if (won)
                {
                    NetData n = new NetData("game_over", "");
                    multi.sendData(JsonConvert.SerializeObject(n));
                    StatSave statUpdate1 = new StatSave(0, 1, 0);
                    StatSave.Add(statUpdate1);
                }
                StatSave statUpdate = new StatSave(0, 0, 1);
                StatSave.Add(statUpdate);
                multi.ReturnToLobby();
            }
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }
        StatSave statUpdate = new StatSave(1, 0, 0);
        StatSave.Add(statUpdate);
        transform.position = new Vector3(0, -500, 0);
        currentHealth = maxHealth;
        healthBar.value = (0.0f + currentHealth) / maxHealth;
        isDead = true;
        respawnCounter = respawnTime;
        if (!beaconManager.IsBeaconAlive(color))
        {
            respawnCounter = float.PositiveInfinity;
        }
        respawnScreen.SetActive(true);
    }
    private void Respawn()
    {
        transform.position = spawn.transform.position;
        respawnScreen.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            canJump = true;
        }
    }
    private void sendAnimation(string key, string value)
    {
        if (multi.gameStarted)
        {
            NetObject obj = GetComponent<NetObject>();
            if (obj != null)
            {
                NetData n = new NetData("animation", key);
                n.key = value;
                n.id = obj.id;
                multi.sendData(JsonConvert.SerializeObject(n));
            }
        }
    }
    public void Damage(int amount)
    {
        currentHealth -= amount;
        healthBar.value = (0.0f + currentHealth) / maxHealth;
        Debug.Log("Damage: " + amount + " Health: " + currentHealth);
    }
    public void addKnockback(Vector3 from)
    {
        Debug.Log("Knockback");
        Vector3 direction = transform.position - from;
        direction.Normalize();
        direction.y = 0.5f;
        direction *= 2000;
        rb.AddForce(direction);
        knockedBack = true;
        knockbackCounter = 0.5f;
    }
}
