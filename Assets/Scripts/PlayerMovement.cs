// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : MonoBehaviour {
    
    // Assingables
    public Transform playerCam;
    public Transform orientation;

    // Keybinds
    [Header("Keybinds")] 
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    
    // Stats
    public float maxHealth = 100f;
    public float health = 100f;
    public bool isDead = false;
    
    // Movement
    [Header("Movement")] 
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    
    // Jumping
    [Header("Jumping")]
    public float jumpForce;
    private bool readyToJump;
    public float airMultiplier;

    // Crouching
    [Header("Crouching")] 
    public float crouchSpeed;
    public float crouchHeight;
    private float startYScale;
    
    // Ground Check
    [Header("Ground Check")] 
    public float playerHeight;
    public float playerRadius;
    public LayerMask ground;
    private bool grounded;
    
    // Slope Handling
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    
    // Input
    private float horizontalInput;
    private float verticalInput;

    // Other
    private Rigidbody rb;
    private Vector3 moveDirection;
    public MovementState state;
    public GameObject blood;
    public HealthBar healthBar;
    public enum MovementState
    {
        WALKING,
        SPRINTING,
        CROUCHING,
        AIR
    }
    
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

        // Init jumping
        readyToJump = true;
        
        // Init player Y scale
        startYScale = transform.localScale.y;

        // Init player health
        health = maxHealth;
    }
    
    private void Update() 
    {
        MyInput();
        Look();
        GroundCheck();
        SpeedControl();
        StateHandler();
    }

    private void FixedUpdate() 
    {
        Movement();
    }

    // User input (should be it's own class)
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            exitingSlope = true;
            
            Jump();
        }
        
        Crouch();
    }
    
    // Player movement state machine
    private void StateHandler()
    {
        // Mode - Crouching
        if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.CROUCHING;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey) && CanStandUp())
        {
            state = MovementState.SPRINTING;
            moveSpeed = sprintSpeed;
        }
        
        // Mode - Walking
        else if (grounded && !Input.GetKey(crouchKey) && CanStandUp())
        {
            state = MovementState.WALKING;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.AIR;
        }
    }
    // Player movement with WASD
    private void Movement() 
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // on ground
        else if (grounded)
        { 
            rb.AddForce(10f * moveSpeed * moveDirection, ForceMode.Force);
        }
        
        // in air
        else if (!grounded)
        {
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection, ForceMode.Force);
        }
        
        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }
    
    // FPS camera control with mouse
    private void Look() 
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        // find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        // rotate, and also make sure we dont over- or under-rotate
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    // Casts raycast to ground to check if player is on Layer "Ground"
    private void GroundCheck()
    {
        if (state == MovementState.CROUCHING)
        {
            // Line cast
            // grounded = Physics.Raycast(transform.position, Vector3.down, crouchYScale * 1.1f, ground);
            // Debug.DrawRay(transform.position, 1.1f * crouchYScale * Vector3.down, Color.blue);
            
            // Capsule cast
            grounded = Physics.CheckCapsule(transform.position, transform.position - new Vector3(0,(crouchHeight * 0.25f + 0.2f), 0), playerRadius, ground);
        }
        else
        {
            // Line cast
            // grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, ground);
            // Debug.DrawRay(transform.position, 0.5f * playerHeight * Vector3.down, Color.red);
            
            // Capsule cast
            grounded = Physics.CheckCapsule(transform.position, transform.position - new Vector3(0,(playerHeight * 0.25f + 0.2f), 0), playerRadius, ground);
        }
        
        // Apply groundDrag if grounded
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    // Limits player speed
    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }

        // limiting speed on ground or in air
        else
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

    // Controls player jumping ability
    private void Jump()
    {
        if (grounded)
        {
            exitingSlope = false;
            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            readyToJump = true;
        }
    }

    // Scales player down when crouching
    private void Crouch()
    {
        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
            rb.AddForce(Vector3.down * 0.5f, ForceMode.Impulse);
        }

        // stop crouch
        if (CanStandUp() && !Input.GetKey(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    // Checks if player can stand up when crouched
    private bool CanStandUp()
    {
        /*// Line cast
        if (Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.7f))
        {
            return false;
        }*/
        
        // Capsule cast
        if (Physics.CheckSphere(transform.position + new Vector3(0,(playerHeight * 0.58f), 0), playerRadius))
        {
            return false;
        }
        
        return true;
    }
    
    // Check if player is on slope at a specified angle
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    
    // Reduces health on damage and kills player
    public void TakeDamage(float damageAmount)
    {
        healthBar.Damage(damageAmount);
        health -= damageAmount;
        
        Instantiate(blood,
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            transform.rotation);

        if (health <= 0)
        {
            this.enabled = false;
            isDead = true;
        }
    }

    // Project move direction based on angle of slope
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // Debug Gizmos for CanStandUp debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, 0.7f * Vector3.up * playerHeight);
        Gizmos.DrawSphere(transform.position + new Vector3(0,(playerHeight * 0.58f), 0), playerRadius);
    }
    public void Heal(float amount)
    {
        healthBar.Heal(amount);
        health += amount;
        
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }
}