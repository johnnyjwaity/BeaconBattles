using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerController : MonoBehaviour {

    public float walkSpeed;
    public float runSpeed;
    public float jumpStrength;
    public GameObject spawn;
    public float yMin;
    public float rotateSensitivity;
    private Rigidbody rb;
    private Animator anim;

    private float atackCount = 0;
    public GameObject beacon;

    private bool canJump = true;

    private Vector3 originalRotation;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        originalRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetBool("run", false);
        anim.SetBool("walk", false);

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

        rb.velocity = new Vector3(newVelocity.x * speed, rb.velocity.y, newVelocity.z * speed);

        

        if (horizontal != 0 || vertical != 0)
        {
            anim.SetBool(movementType, true);
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
            anim.SetTrigger("attack");
            atackCount += 1;
        }
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("stab");
            atackCount += 1;
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
}
