using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f, jumpForce = 15f, projectileSpeed = 10f, projectileCD = .5f;
    [SerializeField] private int JumpCount = 2;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck, shootingPoint;

    private Animator animator;
    private bool isGrounded;
    private float jumps, lastShot = 0;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        lastShot = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        HandleShoot();
        HandleMovement();
        HandleJump();
        UpdateAnimation();
    }
    private void HandleShoot()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            jumps = JumpCount;
        }

        if (Input.GetButtonDown("Fire1") & (Time.time - lastShot) >= projectileCD)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            // Apply forward velocity to the projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            print(shootingPoint);

            if (rb != null)
            {
                rb.linearVelocity = shootingPoint.forward * projectileSpeed;
            }

            lastShot = Time.time;
        }
    }
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (isGrounded) rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0) transform.localScale = new Vector3(1.53f, 1.53f, 1.53f);
        else if (moveInput < 0) transform.localScale = new Vector3(-1.53f, 1.53f, 1.53f);

        Debug.DrawLine(transform.position, transform.position + new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0));

        if (transform.position.y < -100)
        {
            transform.position = new Vector3(0, 10, 0);
            rb.linearVelocity = Vector2.zero;
        }
    }
    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            jumps = JumpCount;
        }

        if (Input.GetButtonDown("Jump") && (isGrounded || (jumps - 1 > 0)))
        {
            jumps -= 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > .1f;
        bool isJumping = !isGrounded;
        bool isAiming = Input.GetButtonDown("Fire1");

        animator.SetBool("Walking", isRunning);
        animator.SetBool("Jumping", isJumping);
        animator.SetBool("Aiming", isAiming);
    }
}
