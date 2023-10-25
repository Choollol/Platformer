using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(InputController))]

public class PlayerMovement : MonoBehaviour
{
    /*
     * Requires child "Ground Check" object at feet/bottom of player object
     * Create layer named "Ground" and add to groundLayers and objects player can jump off of
     * Default paramters: 1 mass, 3 gravity, 12 jump force, 10 speed
     */

    private Rigidbody2D rb;
    private InputController inputController;

    private Transform groundCheck;
    [SerializeField] private List<LayerMask> groundLayers;

    [SerializeField] private float jumpForce;

    private bool isGrounded;
    private bool isJumping;

    private float jumpCounter = 0;

    private int extraJumps = 0;
    private int extraJumpsCounter = 0;
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter = 0;
    private float jumpBuffer = 0.1f;
    private float jumpBufferCounter = 0;

    private Vector2 velocity;

    [SerializeField] private float speed;

    private Vector2 additionalForce;
    private Vector2 additionalForceDecrement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputController = GetComponent<InputController>();

        groundCheck = transform.GetChild(0);

        CameraController.Instance.SetTarget(gameObject);
    }
    private void FixedUpdate()
    {
        MovementUpdate();
    }
    void Update()
    {
        JumpUpdate();

        additionalForce -= additionalForceDecrement;
    }
    private void MovementUpdate()
    {
        velocity = new Vector3(inputController.horizontalInput, 0) * speed;

        rb.velocity = new Vector2(velocity.x, rb.velocity.y) + additionalForce;
    }
    private void JumpUpdate()
    {
        float dt = Time.deltaTime;

        // Coyote time
        if (isGrounded || extraJumpsCounter < extraJumps)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= dt;
        }

        // Jump buffer
        if (inputController.doJump)
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= dt;
        }

        // Jump
        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0 && (!isJumping || jumpCounter > 0.01f))
        {
            // Jump counter to prevent multiple jump button forces
            if (isJumping)
            {
                jumpCounter += dt;
            }

            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpBufferCounter = 0;
            if (!isGrounded)
            {
                extraJumpsCounter++;
            }
            isGrounded = false;
            isJumping = true;
            //AudioManager.PlaySound("Jump Sound");
            jumpCounter = 0;
        }
        // Variable jump height
        if (!inputController.isJumpHeld && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.96f);

            coyoteTimeCounter = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Grounded
        foreach (LayerMask groundLayer in groundLayers)
        {
            if (1 << collision.gameObject.layer == groundLayer && collision.transform.position.y < groundCheck.position.y + 0.1f)
            {
                isGrounded = true;
                extraJumpsCounter = 0;
                isJumping = false;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ungrounded
        foreach (LayerMask groundLayer in groundLayers)
        {
            if (1 << collision.gameObject.layer == groundLayer)
            {
                isGrounded = false;
            }
        }
    }
}
