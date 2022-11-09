// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : MonoBehaviour {

    // Assingables
    public Transform playerCam;
    public Transform orientation;

    // Movement
    [Header("Movement")] public float moveSpeed;
    public float groundDrag;

    // Ground Check
    [Header("Ground Check")] public float playerHeight;
    public LayerMask ground;
    private bool grounded;
    
    // Input
    private float horizontalInput;
    private float verticalInput;
    
    // Other
    private Rigidbody rb;
    private Vector3 moveDirection;

    // Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;
    private float desiredX;
    
    void Awake() 
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        // Get rigidbody and freeze rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update() 
    {
        MyInput();
        Look();
        GroundCheck();
        SpeedControl();
    }

    private void FixedUpdate() 
    {
        Movement();
    }

    // User Input (should be it's own class)
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Movement() 
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(10f * moveSpeed * moveDirection, ForceMode.Force);
    }
    
    private void Look() 
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}