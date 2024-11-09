
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("References")]
    public Transform playerCamTransform;
    public GameObject cameraHolder;
    public Transform orientation;
    public Transform playerHead;
    CapsuleCollider playerCollider;

    private Rigidbody rb;
    WallRunning wallRunning;
    ResetPosition resetPos;

    [Header("Movement")]
    [SerializeField]
    LayerMask whatIsGround;
    public float maxSpeed;
    float moveSpeed = 3000;
    bool grounded;
    float counterMovement = 0.1f;
    float threshold = 0.01f;
    float maxSlopeAngle = 45f;

    [Header("Crouch and Slide")]
    [SerializeField]
    float slideForce = 100;

    [SerializeField]
    float maxSlide = 1.5f;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    float slideCounterMovement = 0.1f;
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    float slideTimer;

    [Header("Jumping")]
    [SerializeField]
    float jumpForce = 350f;

    [SerializeField]
    int maxJumps = 1;

    bool readyToJump = true;
    float jumpCooldown = 0.25f;
    public int jumps = 0;

    [Header("Input")]
    float x, y;
    public bool isWallRunning { get; set; }
    public bool isIdle { get; set; }
    public bool jumping { get; set; }
    public bool sprinting { get; set; }
    public bool crouching { get; set; }
    public bool isSliding { get; set; }
    public bool isCrouching { get; set; }
    public bool isJumping { get; set;}

    [Header("Bool")]
    bool isPaused;
    bool waitingToStand;
    bool doubleJump = true;

    [Header("Animations")]
    [SerializeField]Animator anim;

    [Header("UI")]

    [SerializeField]
    public MoveState state;
    public enum MoveState
    {
        wallRunning,
        sprinting,
        crouching,
        sliding,
        jumping,
        idle,
        air
    }

    void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        wallRunning = GetComponent<WallRunning>();
        resetPos = GetComponent<ResetPosition>();

        playerCollider = GetComponent<CapsuleCollider>();
    }
    
    void Start() 
    {
        slideTimer = maxSlide;
    }

    
    private void FixedUpdate() 
    {
        if(GetPlayerMovement() == Vector2.zero)
        {
            isIdle = true;

        }
        
        if(GetPlayerMovement() != Vector2.zero && !GameManager.Instance.waitingForCountdown)
        {
            isIdle = false;
            Movement();
        }
        
        if(isSliding)
        {
            SlideMovement();
        }
        
    }

    private void Update() 
    {

        if (GameManager.Instance.isPaused)
        {
            return;
        }
        MyInput();
        StateHandler();

        sprinting = state == MoveState.sprinting ? sprinting = true : sprinting = false;

    }


    void StateHandler()
    {
        // Handle all state transitions
        if (isSliding) SetState(MoveState.sliding);
        else if(isJumping) SetState(MoveState.jumping, 30f);
        else if (IsAirborne()) SetState(MoveState.air, 30f);
        else if (isWallRunning) SetState(MoveState.wallRunning, 32f);
        else if (isCrouching) SetState(MoveState.crouching, 12f);
        else if (isIdle) SetState(MoveState.idle) ;
        else SetState(MoveState.sprinting, 40f);
    }

    // Extract conditions into methods for readability
    private bool IsAirborne()
    {
        return !grounded && !isWallRunning;
    }

    // Helper method to set state and maxSpeed
    private void SetState(MoveState newState, float speed = 0f)
    {
        state = newState;
        if (speed > 0f) maxSpeed = speed;
    }
    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        x = GetPlayerMovement().x;
        y = GetPlayerMovement().y;
        jumping = UserInputs.inputREF.jumpInput;
        crouching = UserInputs.inputREF.crouchInput;

        anim.SetFloat("moveX", x);
        anim.SetFloat("moveY", y);

        if (UserInputs.inputREF.crouchInput && rb.velocity.magnitude > 10f && slideTimer > 0 && !GameManager.Instance.waitingForCountdown)
        {
            StartSlide();
        }
        
        if(UserInputs.inputREF.crouchInput && rb.velocity.magnitude < 15f && !GameManager.Instance.waitingForCountdown)
        {
            StartCrouch();
        }

        if (isCrouching)
        {
            Crouching();
        }

        else if (waitingToStand || UserInputs.inputREF.notCrouchInput)
        {
            StopCrouch();
        }
        else if (UserInputs.inputREF.notCrouchInput && rb.velocity != Vector3.zero && slideTimer <= 0)
        {
            StopSlide();
        }
    }

    void StartSlide()
    {
        if (!isSliding)
        {
            isSliding = true;
            playerCollider.height = .5f;
        }
    }

    void SlideMovement()
    {
        slideTimer -= Time.deltaTime;
        if (slideTimer > 0)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
        else
        {
            StopSlide();
        }
    }

    void StopSlide()
    {
        if (isSliding)
        {
            slideTimer = maxSlide;
            isSliding = false;

            // Check if there is an obstacle above the player
            if (Physics.Raycast(transform.position, Vector3.up, 1f))
            {
                waitingToStand = true;
                
                StartCrouch();
            }
            else
            {
                // If there is nothing above, stand up
                playerCollider.height = 2;
            }
        }
    }

    private void StartCrouch()
    {
        isCrouching = !isCrouching;
    }

    void Crouching()
    {
        playerCollider.height = .5f;
    }

    private void StopCrouch()
    {
        // Only uncrouch if there is nothing above the player
        if (!Physics.Raycast(playerHead.position, Vector3.up, 2f))
        {
            playerCollider.height = 2;
            waitingToStand = false;
            isCrouching = false;
        }
    }

    private void Movement() 
    {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        //CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && UserInputs.inputREF.jumpFloat > 0)
        { 
            Jump();
        } 
        
        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (isSliding && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && isSliding) multiplierV = 0f;

        // Use the camera's forward and right directions for movement
        Vector3 moveDirection = (playerCamTransform.forward * y) + (playerCamTransform.right * x);
        moveDirection.y = 0f;  // Ensure we don't apply vertical movement based on camera angle
        moveDirection = moveDirection.normalized;

        // Apply forces to move player
        rb.AddForce(moveDirection * moveSpeed * Time.deltaTime * multiplier * multiplierV);
    }

    private void Jump() 
    {
        
        if (readyToJump && jumps < maxJumps)
        {
            isJumping= true;
            readyToJump = false;

            anim.SetTrigger("Jump");
            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            jumps++;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
        isJumping = false;
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    
    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));

                jumps = 0;
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "End")
        {   
            if(GameManager.Instance.GetTiming() != 0)
            {
                GameManager.Instance.StopTimer();
            }
            else
            {
                this.GetComponent<ResetPosition>().ResetPos();
            }
           

        }
    }

    

    public Vector2 GetPlayerMovement()
    {
        return UserInputs.inputREF.moveInput;

    }

}
