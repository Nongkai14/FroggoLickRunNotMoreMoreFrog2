//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(Rigidbody))]
//public class MovementStateManager : MonoBehaviour
//{
//    [SerializeField] private float moveSpeed = 3f;
//    [SerializeField] private float groundCheckDistance = 0.2f;
//    [SerializeField] private LayerMask groundMask;
//    [SerializeField] private float gravity = -9.81f;

//    private Rigidbody rb;
//    private PlayerInputActions inputActions;
//    private Vector2 inputMovement;
//    private Vector3 movementDirection;

//    private bool isGrounded;
//    private Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0); // ปรับตามขนาดวัตถุ
//    private float verticalVelocity;

//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.freezeRotation = true;

//        inputActions = new PlayerInputActions();
//    }

//    private void OnEnable()
//    {
//        inputActions.Player.Enable();
//        inputActions.Player.Move.performed += OnMove;
//        inputActions.Player.Move.canceled += OnMove;
//    }

//    private void OnDisable()
//    {
//        inputActions.Player.Move.performed -= OnMove;
//        inputActions.Player.Move.canceled -= OnMove;
//        inputActions.Player.Disable();
//    }

//    private void OnMove(InputAction.CallbackContext context)
//    {
//        inputMovement = context.ReadValue<Vector2>();
//    }

//    private void FixedUpdate()
//    {
//        CheckGrounded();
//        ApplyGravity();
//        Move();
//    }

//    private void Move()
//    {
//        movementDirection = transform.forward * inputMovement.y + transform.right * inputMovement.x;
//        Vector3 velocity = movementDirection * moveSpeed;
//        velocity.y = verticalVelocity;
//        rb.velocity = velocity;
//    }

//    private void CheckGrounded()
//    {
//        Vector3 checkPosition = transform.position - groundCheckOffset;
//        isGrounded = Physics.Raycast(checkPosition, Vector3.down, groundCheckDistance, groundMask);
//    }

//    private void ApplyGravity()
//    {
//        if (isGrounded && verticalVelocity < 0)
//        {
//            verticalVelocity = -2f;
//        }
//        else
//        {
//            verticalVelocity += gravity * Time.fixedDeltaTime;
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = isGrounded ? Color.green : Color.red;
//        Vector3 checkPosition = transform.position - groundCheckOffset;
//        Gizmos.DrawLine(checkPosition, checkPosition + Vector3.down * groundCheckDistance);
//    }
//}
