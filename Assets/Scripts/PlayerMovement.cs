using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*
     * Requires child "Ground Check" object at feet/bottom of player object
     * Create layer named "Ground" and add to groundLayers and objects player can jump off of
     */

    private Rigidbody2D rb;

    private Transform groundCheck;
    [SerializeField] private List<LayerMask> groundLayers;
    [SerializeField] private float groundCheckRadius;

    [SerializeField] private float jumpForce;

    private bool isGrounded;
    private bool isJumping;

    private int extraJumps = 0;
    private int extraJumpsCounter = 0;
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter = 0;
    private float jumpBuffer = 0.1f;
    private float jumpBufferCounter = 0;

    private Vector2 velocity;

    private float horizontalInput;
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
        horizontalInput = Input.GetAxisRaw("Horizontal");

        //GroundedUpdate();
        JumpUpdate();

        additionalForce -= additionalForceDecrement;
    }
    private void MovementUpdate()
    {
        velocity = new Vector3(horizontalInput, 0) * speed;

        rb.velocity = new Vector2(velocity.x, rb.velocity.y) + additionalForce;
        //transform.position += new Vector3(velocity.x * Time.fixedDeltaTime, 0);
        //rb.AddForce(new Vector2(velocity.x, 0));
    }

    /*private void GroundedUpdate()
    {
        foreach (LayerMask groundLayer in groundLayers) 
        {
            if (Physics2D.Raycast(groundCheck.transform.position, Vector2.down, groundCheckRadius, groundLayer)
                && jumpTimer > jumpCooldown)
            {
                isGrounded = true;
                isJumping = false;
                extraJumpsCounter = 0;
            }
        }
    }*/
    private void JumpUpdate()
    {
        float dt = Time.deltaTime;

        if (isGrounded || extraJumpsCounter < extraJumps)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= dt;
        }

        if (Input.GetButtonDown("Jump"))
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
            isJumping = true;
            //AudioManager.PlaySound("Jump Sound");
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
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
