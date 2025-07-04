using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.5f;
    private bool readyToJump = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    private Vector2 moveInput;
    private Rigidbody rb;

    [HideInInspector]public bool freeze;
    bool activeGrapple;
    bool enableMovementOnNextTouch;

    [Header("For UI")]
    public float hp;
    private Animator animator;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        freeze = false;
        rb.freezeRotation = true;
        hp = 100f;
        animator = GetComponent<Animator>();
        Time.timeScale = 1f;
    }

    private void Update()
    {
        Debug.Log("Move Input: " + moveInput);
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround);

        // Apply drag only when grounded
        rb.linearDamping = grounded && !activeGrapple ? groundDrag : 0f;

        SpeedControl();

        if (freeze)
        {
            if (activeGrapple) return;
            rb.linearVelocity = Vector3.zero;
        }

        UpdateAnimationStates();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;

        Vector3 moveDir = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (grounded)
            rb.AddForce(moveDir * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDir * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Debug.Log("Jump input: " + context);
        //Debug.Log("ReadyToJump: " + readyToJump);
        //Debug.Log("Grounded: " + grounded);

        if (context.performed && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void hookToPosition(Vector3 targetposition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetposition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRetrictions), 3f);
    }

    private Vector3 velocityToSet;

    private void SetVelocity()
    {
        rb.linearVelocity = velocityToSet;
        enableMovementOnNextTouch = true;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public void ResetRetrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRetrictions();

            GetComponent<Grabling>().StopGrapple();
        }
    }
    public bool IsGrounded()
    {
        return grounded;
    }

    private void UpdateAnimationStates()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;

        animator.SetFloat("Speed", speed);              // ควบคุมวิ่ง/idle
        animator.SetBool("isJumping", !grounded);       // ควบคุม jump
    }
    public void ResetPlayerState()
    {
        freeze = false;
        activeGrapple = false;
        enableMovementOnNextTouch = false;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        hp = 100f;
    }

}
