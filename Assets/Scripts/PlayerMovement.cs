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
    //private bool isJumping;

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

        groundCheck = transform.GetChild(0);
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
        bool doJump = inputController.doJump;

        if (isGrounded || extraJumpsCounter < extraJumps)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= dt;
        }

        if (doJump)
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= dt;
        }

        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0)
        {
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
            //isJumping = true;
            //AudioManager.PlaySound("Jump Sound");
        }
        if (doJump && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (LayerMask groundLayer in groundLayers)
        {
            if (1 << collision.gameObject.layer == groundLayer && collision.transform.position.y < groundCheck.position.y)
            {
                isGrounded = true;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        foreach (LayerMask groundLayer in groundLayers)
        {
            if (1 << collision.gameObject.layer == groundLayer)
            {
                isGrounded = false;
            }
        }
    }
}
