using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;    // Speed of horizontal movement
    public float jumpForce = 10f;   // Force applied when jumping

    [Header("Ground Check")]
    public Transform groundCheckPoint; // Assign an empty GameObject positioned at the player's feet
    public float groundCheckRadius = 0.2f; // Radius of the circle used to detect ground
    public LayerMask groundLayer;     // Set this in the inspector to identify what layers are considered ground

    [Header("Components")]
    private Rigidbody2D rb;             // Reference to the Rigidbody2D component for physics
    private Animator animator;          // Reference to the Animator component for animations
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for flipping the sprite
    private AudioSource audioSource;    // Reference to the AudioSource for playing sounds (optional)

    [Header("Animation Parameters")]
    private string isWalkingParameter = "isWalking";         // Name of the boolean parameter in the Animator for walking
    private string isGroundedAnimParameter = "isGrounded_anim"; // Name of the boolean parameter in the Animator for being grounded

    [Header("Audio Clips (Optional)")]
    public AudioClip jumpSound;
    public AudioClip walkSound; // Placeholder for walk sounds

    // Private state variables
    private bool isGrounded;            // Is the player currently touching the ground?
    private float horizontalInput;      // Stores the raw horizontal input value (-1, 0, or 1)
    private bool isFacingRight = true;  // Tracks the direction the player is currently facing

    private PlayerInputActions playerInputActions; // Instance of our generated Input Actions class

    // Awake is called when the script instance is being loaded (before Start)
    void Awake()
    {
        // Get references to necessary components attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // If no AudioSource component exists, add one (optional, for sound effects)
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Safety check for groundCheckPoint assignment
        if (groundCheckPoint == null)
        {
            Debug.LogError("Ground Check Point not assigned in the Inspector on PlayerMovement script!");
        }

        // Initialize the Input Actions
        playerInputActions = new PlayerInputActions();
    }

    // This function is called when the object becomes enabled and active
    private void OnEnable()
    {
        playerInputActions.Player.Enable(); // Enable the "Player" action map (or whatever you named it)
    }

    // This function is called when the behaviour becomes disabled or inactive
    private void OnDisable()
    {
        playerInputActions.Player.Disable(); // Disable the "Player" action map to prevent errors
    }

    // Update is called once per frame
    void Update()
    {
        // --- Read Player Input ---
        // Read the horizontal movement value from the "Move" action (Vector2, we take the x component)
        horizontalInput = playerInputActions.Player.Move.ReadValue<Vector2>().x;

        // Check if the "Jump" action was pressed this frame and if the player is grounded
        if (playerInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            Jump();
        }

        // --- Animation & Visual Updates ---
        UpdateWalkingAnimationAndFlipSprite();
    }

    // FixedUpdate is called at a fixed interval, ideal for physics calculations
    void FixedUpdate()
    {
        // --- Ground Check ---
        // Perform a circle overlap check at the groundCheckPoint's position to see if it's overlapping with the groundLayer
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // --- Update Animator with Grounded State ---
        if (animator != null)
        {
            animator.SetBool(isGroundedAnimParameter, isGrounded);
        }

        // --- Apply Movement ---
        // Set the Rigidbody's velocity for horizontal movement, maintaining current vertical velocity
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // Handles the jump action
    void Jump()
    {
        // Apply an upward force for jumping by directly setting the y velocity
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        PlaySound(jumpSound); // Play the jump sound if assigned
    }

    // Updates the walking animation and flips the sprite based on movement direction
    void UpdateWalkingAnimationAndFlipSprite()
    {
        // Update the "isWalking" parameter in the Animator
        if (animator != null)
        {
            bool isCurrentlyWalking = horizontalInput != 0;
            animator.SetBool(isWalkingParameter, isCurrentlyWalking);
        }

        // Flip the sprite's direction
        if (horizontalInput > 0 && !isFacingRight) // Moving right, but facing left
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight) // Moving left, but facing right
        {
            Flip();
        }
    }

    // Flips the character's sprite horizontally
    void Flip()
    {
        isFacingRight = !isFacingRight;             // Toggle the facing direction state
        spriteRenderer.flipX = !spriteRenderer.flipX; // Toggle the SpriteRenderer's flipX property
    }

    // Plays an audio clip using the AudioSource component
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // PlayOneShot is good for short, non-looping sounds like jump
        }
    }

    // This function is called by Unity when the an object is selected in the editor.
    // It draws Gizmos that are pickable and always drawn.
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
            return; // Don't draw if the groundCheckPoint isn't assigned

        // Draw a red wire sphere at the groundCheckPoint's position with the groundCheckRadius
        // This helps visualize the ground check area in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("CollectibleItem"))
        {
            Debug.Log("Collected an item! (Collision)"); // Placeholder for adding to inventory
            Destroy(collision.gameObject);
        }
    }
}

    