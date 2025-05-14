using System;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (move > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        if (move < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void HandleJump()
    {
        // Более точная проверка нахождения на земле
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + groundCheckOffset, groundCheckRadius, groundLayer);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0.1f);
        animator.SetBool("isJump", !isGrounded);
    }

    // Для визуализации точки проверки земли в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + groundCheckOffset, groundCheckRadius);
    }
}