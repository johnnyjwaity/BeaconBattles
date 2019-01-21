using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerController : MonoBehaviour {

    public float walkSpeed;
    public float runSpeed;
    public float jumpStrength;
    public GameObject[] spawnPoints;
    private GameObject spawn;
    public float yMin;
    public float rotateSensitivity;
    private Rigidbody rb;
    private Animator anim;

    private float atackCount = 0;
    public GameObject beacon;

    private bool canJump = true;

    private Vector3 originalRotation;

    private Multiplayer multi;
    public Attack[] attacks;
    private bool knockedBack;
    private float knockbackCounter = 0;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        originalRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        multi = FindObjectOfType<Multiplayer>();
        Cursor.visible = false;
        FindObjectOfType<Multiplayer>().syncQueue.Add(gameObject);
        Debug.Log("Color: " + PlayerPrefs.GetInt("color"));
        spawn = spawnPoints[PlayerPrefs.GetInt("color")];
        transform.position = spawn.transform.position;
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
            transform.position = spawn.transform.position;
        }
        if (Input.GetMouseButtonDown(0))
        {
            sendAnimation("attack", "trigger");
            anim.SetTrigger("attack");
            atackCount += 1;
            attacks[0].run();
        }
        if (Input.GetMouseButtonDown(1))
        {
            sendAnimation("stab", "trigger");
            anim.SetTrigger("stab");
            atackCount += 1;
            attacks[1].run();
        }

        if(atackCount >= 2)
        {
            Destroy(beacon, 1);
            atackCount = 0;
        }
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
