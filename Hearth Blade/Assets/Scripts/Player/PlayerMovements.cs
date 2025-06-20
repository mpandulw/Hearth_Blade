using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    private float horizontal;

    // Input system
    private InputSystem_Actions inputActions;

    // Move Input
    private Vector2 moveInput;
    private float mobileMoveInputX;

    // Movements Settings
    [Header("Movements Setting")]
    public float movSpeed;
    public float acceleration = 40f;

    // Jump Settings
    [Header("Jump Settings")]
    public float jumpForce;
    [SerializeField] private LayerMask layerMask;

    public bool isJumping = false;

    // Dust effect
    public ParticleSystem dust;

    // Wallslide mechanic
    public bool isWallsliding;
    public float wallslidingSpeed;
    public float wallslideDuration;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingForce = new(8f, 16f);

    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private LayerMask wallLayer;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCancel;
        inputActions.Player.Jump.performed += OnJump;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Check input
        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileMoveInputX, 0f);
        }
        else
        {
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        }

        horizontal = moveInput.x;

        float targetSpeed = moveInput.x * movSpeed;
        float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.deltaTime);

        Vector2 newVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;

        if (isGrounded() && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            isJumping = false;
        }

        UpdateAnimation();

        Wallslide();
    }

    private enum MovementState { idle, run, jump, fall }
    private void UpdateAnimation()
    {
        MovementState state;
        // Run state
        if (rb.linearVelocity.x > 0f)
        {
            state = MovementState.run;
            sprite.flipX = false;
            if (!isJumping)
            {
                if (!dust.isPlaying) dust.Play();

            }
        }
        else if (rb.linearVelocity.x < 0f)
        {
            state = MovementState.run;
            sprite.flipX = true;
            if (!isJumping)
            {
                if (!dust.isPlaying) dust.Play();
            }
        }
        else
        {
            state = MovementState.idle;
            if (dust.isPlaying) dust.Stop();
        }

        // Jump State
        if (rb.linearVelocity.y > 0.1f)
        {
            state = MovementState.jump;
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            state = MovementState.fall;
        }

        anim.SetInteger("state", (int)state);
    }

    private void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    private void OnMoveCancel(InputAction.CallbackContext context) => moveInput = Vector2.zero;
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded())
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void mobileMoveLeft(bool pressed)
    {
        if (pressed)
            mobileMoveInputX = -1f;
        else if (mobileMoveInputX == -1f)
            mobileMoveInputX = 0f;
    }
    public void mobileMoveRight(bool pressed)
    {
        if (pressed)
            mobileMoveInputX = 1f;
        else if (mobileMoveInputX == 1f)
            mobileMoveInputX = 0f;
    }
    public void mobileJump()
    {
        if (isGrounded())
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, layerMask);
    }


    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    private void Wallslide()
    {
        if (IsWalled() && !isGrounded() && horizontal != 0f)
        {
            isWallsliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallslidingSpeed, float.MaxValue));
        }
        else
        {
            isWallsliding = false;
        }
    }
}
