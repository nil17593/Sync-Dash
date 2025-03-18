using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] private float baseJumpHeight = 2.0f;
    [SerializeField] private float baseJumpDuration = 0.5f;
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool isJumping;
    private float jumpStartTime;
    private float initialY;
    private float jumpDuration;
    private float jumpHeight;
    private float fallMultiplier = 1.3f;

    private void Start()
    {
        initialY = transform.position.y;
    }

    void Update()
    {
        // Check if the cube is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle jump input (Spacebar or other input)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            StartJump();
        }

        // Continue the jump if it's active
        if (isJumping)
        {
            ContinueJump();
        }
    }

    void StartJump()
    {
        // Initialize jump variables
        isJumping = true;
        jumpStartTime = Time.time;

        // Adjust jump timing based on platform speed
        float speedRatio = PlatformController.instance.GetSpeedRatio() / PlatformController.instance.minSpeed;
        jumpDuration = baseJumpDuration / speedRatio;
        jumpHeight = baseJumpHeight;
    }

    void ContinueJump()
    {
        // Calculate the progress of the jump based on elapsed time
        float elapsedTime = Time.time - jumpStartTime;
        float jumpProgress = elapsedTime / jumpDuration;

        if (jumpProgress >= 1f)
        {
            EndJump();
        }
        else
        {
            float verticalPosition;

            if (jumpProgress < 0.5f)
            {
                // Rising part of the jump - use easeOutQuad for a quick initial rise
                float riseProgress = jumpProgress * 2.0f; // Scale to 0-1 range
                verticalPosition = jumpHeight * (1 - (1 - riseProgress) * (1 - riseProgress));
            }
            else
            {
                // Falling part of the jump - use easeInQuad with a multiplier for faster falling
                float fallProgress = (jumpProgress - 0.5f) * 2.0f; // Scale to 0-1 range
                fallProgress = Mathf.Min(fallProgress * fallMultiplier, 1.0f); // Apply fall multiplier
                verticalPosition = jumpHeight * (1.0f - (fallProgress * fallProgress));
            }

            // Update the position
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, initialY + verticalPosition, currentPos.z);
        }
    }

    void EndJump()
    {
        isJumping = false;

        // Make sure the cube ends at the correct height
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, initialY, currentPos.z);
    }

    public ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Orb"))
        {
            particle.Play();
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       
    }
}