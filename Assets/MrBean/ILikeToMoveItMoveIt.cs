using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ILikeToMoveItMoveIt : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] float movementSpeed; 
    [SerializeField] float accFactor;
    [SerializeField] float groundDrag;
    Rigidbody rb;

    [Header("Jumping")]

    [SerializeField] float jumpForce;
    [SerializeField] float airMultiplier;
    public KeyCode jumpKey = KeyCode.Space;

    float jumpCooldown = 0.2f;
    bool readyToJump;

    float horizontalInput;
    float verticalInput;
    UnityEngine.Vector3 moveDirection;

    [Header("Ground Check")]

    [SerializeField] Collider boxCastCollider;
    RaycastHit boxCasthit;
    [SerializeField] float boxCastMaxDistance;
    public LayerMask whatIsGround; //Baby don't hurt me
    public bool grounded;


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    // Update is called once per frame
    private void Update()
    {
        grounded = Physics.BoxCast(boxCastCollider.bounds.center, transform.localScale * 0.99f, -transform.up, out boxCasthit, transform.rotation, boxCastMaxDistance);//new UnityEngine.Vector3(0,-1,0)

        MyInput();
        SpeedControl();
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        
    }
    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if(grounded)
            rb.AddForce(accFactor * moveDirection.normalized, ForceMode.Force);
        else
            rb.AddForce(accFactor * airMultiplier * moveDirection.normalized, ForceMode.Force);
        
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void SpeedControl ()
    {
        UnityEngine.Vector3 flatVel = new UnityEngine.Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > movementSpeed)
        {
            UnityEngine.Vector3 limitedVel = flatVel.normalized * movementSpeed + UnityEngine.Vector3.up* rb.velocity.y;
            rb.velocity = limitedVel;
        }
    }
    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
