
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WallRunning : MonoBehaviour
{
    [Header("References")]
    CamHolder camHolder;
    Transform orient;
    PlayerMovement movement;
    Rigidbody rb;

    [Header("Layer")]
    [SerializeField]
    LayerMask Wall;
    [SerializeField]
    LayerMask Ground;

    [Header("Wallrun Forces")]
    public float wallRunForce;
    float wallRunGravity = 1f;

    [Header("Wallrun Jump")]
    [SerializeField]
    float wallJumpUpForce;
    [SerializeField]
    float wallJumpSideForce;
    [SerializeField]
    float gravityCounterForce;

    float wallCheckDistance = 1f;
    float minJumpHeight = 2f;
    RaycastHit leftWallHit, rightWallHit;
    bool wallLeft, wallRight;
    bool useGravity = true;

    bool readyToWallRun;


    [Header("Exiting")]
    private bool exitingWall;
    float exitWallTime;
    private float exitWallTimer;

    Vector3 startRotate;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        orient = movement.orientation;
        camHolder = movement.cameraHolder.GetComponent<CamHolder>();

        startRotate = new Vector3 (transform.rotation.x, transform.rotation.y, transform.rotation.z);
    }

    void Update()
    {
        CheckWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (movement.isWallRunning)
        {
            WallRunMovement();
        }
    }

    void CheckWall()
    {
        wallRight = Physics.Raycast(transform.position, orient.right, out rightWallHit, wallCheckDistance, Wall);
        wallLeft = Physics.Raycast(transform.position, -orient.right, out leftWallHit, wallCheckDistance, Wall);
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }

    void StateMachine()
    {
        Vector2 move = GetPlayerMovement();

        if ((wallLeft || wallRight) && move.y > 0 && AboveGround())
        {
            if (!movement.isWallRunning)
            {
                StartWallRun();
            }

            if (UserInputs.inputREF.jumpInput)
            {
                WallJump();
            }
        }
        // State 2 - Exiting
        else if (exitingWall)
        {
            if (movement.isWallRunning)
                StopWallRun();
        }

        else
        {
            if (movement.isWallRunning)
            {
                StopWallRun();
            }
        }
    }

    void StartWallRun()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        // Calculate the wall-forward direction based on player velocity and wall normal
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up); // This works better for flat walls
        Vector3 adjustedWallForward = Vector3.ProjectOnPlane(orient.forward, wallNormal).normalized; // Adjust for angled walls

        // Add angle tolerance check
        float angleBetween = Vector3.Angle(orient.forward, adjustedWallForward);
        if (angleBetween < 10f) // 30 degrees tolerance
        {
            movement.isWallRunning = true;
            movement.jumps = 0;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            readyToWallRun = true;

            if (wallLeft)
            {
                camHolder.DoTilt(-10);
            }
            else if (wallRight)
            {
                camHolder.DoTilt(10);
            }
        }
    }
    void WallRunMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orient.forward - wallForward).magnitude > (orient.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Clamp speed along the wall-forward direction
        Vector3 velocityAlongWall = Vector3.Project(rb.velocity, wallForward);
        float maxWallRunSpeed = movement.maxSpeed;
        if (velocityAlongWall.magnitude > maxWallRunSpeed)
        {
            rb.velocity = velocityAlongWall.normalized * maxWallRunSpeed + Vector3.ProjectOnPlane(rb.velocity, wallForward);
        }

        // Apply gravity effect during wall run
        rb.AddForce(Vector3.up * wallRunGravity * Time.deltaTime * rb.mass * 100f, ForceMode.Acceleration);

        // Stick to the wall
        if (!(wallLeft && GetPlayerMovement().x > 0) && !(wallRight && GetPlayerMovement().x < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
        if(useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    private void WallJump()
    {
        // enter exiting wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // reset y velocity and add force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }


    public void StopWallRun()
    {
        movement.isWallRunning = false;
        useGravity = true;
        readyToWallRun = false;
        camHolder.DoTilt(0);
        transform.Rotate((startRotate), Space.World);
    }

    public Vector2 GetPlayerMovement()
    {
        return UserInputs.inputREF.moveInput;
    }
}