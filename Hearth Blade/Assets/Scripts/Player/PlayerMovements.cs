using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    private float horizontal;

    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private float mobileMoveInputX;

    [Header("Movements Setting")]
    public float movSpeed;
    public float acceleration = 40f;

    [Header("Jump Settings")]
    public float jumpForce;
    [SerializeField] private LayerMask layerMask;

    [Header("Player Stamina")]
    public float currentStamina;
    public float maxStamina;
    public float staminaNeeded;
    public Slider staminaBar;
    public float regenStamina = 10f;
    public float recoveryDelayAfterDash = 2f;
    private float lastDashTime;
    private float staminaVelocity;
    private bool isRecovering;


    public bool isJumping = false;
    public ParticleSystem dust;

    [Header("Wallslide")]
    public bool isWallsliding;
    public float wallslidingSpeed;
    public float wallSlideDuration = 0.5f;
    public float wallSlideCounter;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    [Header("Dash")]
    public float dashPower = 24f;
    [SerializeField] private bool isDashing;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingForce = new(8f, 8f);
    private bool canDash = true;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;
    private TrailRenderer tr;


    private Vector3 wallCheckInitialLocalPosition;
    private bool isFacingRight = true;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        wallCheckInitialLocalPosition = wallCheck.localPosition;
        tr = GetComponent<TrailRenderer>();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCancel;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Dash.performed += OnDash;
    }

    void Update()
    {
        staminaBar.value = Mathf.SmoothDamp(staminaBar.value, currentStamina, ref staminaVelocity, 0.1f);

        if (isDashing)
        {
            return;
        }

        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileMoveInputX, 0f);
        }
        else
        {
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        }

        horizontal = moveInput.x;

        float targetSpeed = horizontal * movSpeed;
        float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.deltaTime);
        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);

        if (isGrounded() && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            isJumping = false;
        }

        if (!isDashing && !isRecovering && currentStamina < maxStamina)
        {
            if (Time.time - lastDashTime >= recoveryDelayAfterDash)
            {
                StartCoroutine(recoveryStamina());
            }
        }

        UpdateAnimation();
        Wallslide();
        WallJump();
    }

    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;
    }

    private enum MovementState
    {
        idle, run, jump, fall, wallSliding, dash
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (rb.linearVelocity.x > 0f)
        {
            isFacingRight = true;
            state = MovementState.run;
            sprite.flipX = false;

            if (!isJumping && !dust.isPlaying)
                dust.Play();
        }
        else if (rb.linearVelocity.x < 0f)
        {
            isFacingRight = false;
            state = MovementState.run;
            sprite.flipX = true;

            if (!isJumping && !dust.isPlaying)
                dust.Play();
        }
        else
        {
            state = MovementState.idle;
            if (dust.isPlaying) dust.Stop();
        }

        // Flip wallCheck position based on facing
        wallCheck.localPosition = new Vector3(
            isFacingRight ? Mathf.Abs(wallCheckInitialLocalPosition.x) : -Mathf.Abs(wallCheckInitialLocalPosition.x),
            wallCheckInitialLocalPosition.y,
            wallCheckInitialLocalPosition.z
        );

        if (rb.linearVelocity.y > 0.1f)
        {
            state = MovementState.jump;
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            state = MovementState.fall;
        }

        if (isWallsliding)
        {
            if (isWallsliding)
            {
                sprite.flipX = isFacingRight;
                state = MovementState.wallSliding;
            }

        }

        if (isDashing)
        {
            state = MovementState.dash;
        }

        anim.SetInteger("state", (int)state);
    }

    private void DoJump()
    {
        if (isGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
        }
        else if (wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingForce.x, wallJumpingForce.y);
            wallJumpingCounter = 0;

            isFacingRight = !isFacingRight;
            sprite.flipX = !sprite.flipX;

            wallCheck.localPosition = new Vector3(
                isFacingRight ? Mathf.Abs(wallCheckInitialLocalPosition.x) : -Mathf.Abs(wallCheckInitialLocalPosition.x),
                wallCheckInitialLocalPosition.y,
                wallCheckInitialLocalPosition.z
            );

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    private void OnMoveCancel(InputAction.CallbackContext context) => moveInput = Vector2.zero;
    private void OnJump(InputAction.CallbackContext context) => DoJump();
    private void OnDash(InputAction.CallbackContext context) => DoDash();

    public void MobileDash()
    {
        DoDash();
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
        DoJump();
    }

    public bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, layerMask);
    }

    private void WallJump()
    {
        if (isWallsliding)
        {
            isWallJumping = false;
            wallJumpingDirection = isFacingRight ? -1 : 1;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void Wallslide()
    {
        if (IsWalled() && !isGrounded() && horizontal != 0f && wallSlideCounter >= 0)
        {
            isWallsliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallslidingSpeed, float.MaxValue));
            wallSlideCounter -= Time.deltaTime;
        }
        else
        {
            isWallsliding = false;
            if (isWallJumping)
            {
                wallSlideCounter = wallSlideDuration;
            }
            else if (isGrounded())
            {
                wallSlideCounter = wallSlideDuration;
            }
        }
    }

    private void DoDash()
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        if (currentStamina >= staminaNeeded)
        {
            currentStamina -= staminaNeeded;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            anim.SetInteger("state", (int)MovementState.dash);
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0;
            if (isFacingRight)
            {
                rb.linearVelocity = new Vector2(transform.localScale.x * dashPower, 0f);
            }
            else if (!isFacingRight)
            {
                rb.linearVelocity = new Vector2(-transform.localScale.x * dashPower, 0f);
            }
            tr.emitting = true;
            yield return new WaitForSeconds(dashTime);
            tr.emitting = false;
            rb.gravityScale = originalGravity;
            isDashing = false;
            lastDashTime = Time.time;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }

    private IEnumerator recoveryStamina()
    {
        isRecovering = true;

        while (currentStamina < maxStamina)
        {
            // Jika player melakukan dash lagi saat recovery, restart delay
            if (Time.time - lastDashTime < recoveryDelayAfterDash)
            {
                yield return new WaitForSeconds(recoveryDelayAfterDash - (Time.time - lastDashTime));
            }

            // Tetap cek jika dash dilakukan lagi selama loop
            if (Time.time - lastDashTime < recoveryDelayAfterDash)
            {
                continue;
            }

            currentStamina += regenStamina * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            yield return null;
        }

        isRecovering = false;
    }
}
