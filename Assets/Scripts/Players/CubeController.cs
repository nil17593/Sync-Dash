using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] private float baseJumpHeight = 2.0f;
    [SerializeField] private float baseJumpDuration = 0.5f;
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float forwardSpeed = 5f;  // Player forward speed
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GhostController ghostController;
    [SerializeField] private ParticleSystem burstParticle;

    private bool isGrounded;
    private bool isJumping;
    private float jumpStartTime;
    private float initialY;
    private float jumpDuration;
    private float jumpHeight;
    private float fallMultiplier = 1.3f;

    public float currentSpeed;
    public float minSpeed = 5f;
    public float maxSpeed = 15f;
    public float acceleration = 0.2f;

    private void Start()
    {
        initialY = transform.position.y;
        currentSpeed = minSpeed;
        ghostController.Init(baseJumpHeight, baseJumpDuration, fallMultiplier, currentSpeed); // Pass player's current speed

    }
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    void Update()
    {
        if (GameManager.Instance.GameOver)
            return;

        MovePlayerForward();
    }

    // Handle player forward movement
    void MovePlayerForward()
    {
        float speedRatio = currentSpeed / minSpeed;
        float movementDistance = forwardSpeed * speedRatio * Time.deltaTime;

        // Move player forward
        transform.Translate(Vector3.forward * movementDistance);

        // Check if player crossed a threshold and reset
        if (transform.position.z > GameManager.Instance.dist)
        {
            float offset = transform.position.z - 0f;
            EventManager.TriggerWorldResetEvent(offset);
            ResetPlayerPosition();
        }

        // Update score
        ScoreManager.Instance.UpdateScore(transform);

        // Handle speed increase
        HandleSpeedIncrease();

        // Queue the player's position for the ghost to follow
        if (ghostController != null)
        {
            ghostController.QueueMovementPosition(transform.position);  // Send position to the ghost
        }
    }


    void ResetPlayerPosition()
    {
        Vector3 resetPosition = new Vector3(transform.position.x, transform.position.y, 0f);

        // Reset player's position
        transform.position = resetPosition;

        // Queue the same reset for the ghost (with delay)
        if (ghostController != null)
        {
            ghostController.QueueResetPosition(resetPosition);
        }

    }

    void HandleSpeedIncrease()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            GameManager.Instance.currentSpeed = currentSpeed;
        }
    }

    #region Code which is related to jump but we separated logic of jump so no need this for now
    void StartJump()
    {
        // Initialize jump variables
        isJumping = true;
        jumpStartTime = Time.time;

        float speedRatio = currentSpeed / minSpeed;
        jumpDuration = baseJumpDuration / speedRatio;
        jumpHeight = baseJumpHeight;
    }

    void ContinueJump()
    {
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
                float riseProgress = jumpProgress * 2.0f;
                verticalPosition = jumpHeight * (1 - (1 - riseProgress) * (1 - riseProgress));
            }
            else
            {
                float fallProgress = (jumpProgress - 0.5f) * 2.0f;
                fallProgress = Mathf.Min(fallProgress * fallMultiplier, 1.0f);
                verticalPosition = jumpHeight * (1.0f - (fallProgress * fallProgress));
            }

            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, initialY + verticalPosition, currentPos.z);
        }
    }

    void EndJump()
    {
        isJumping = false;

        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, initialY, currentPos.z);
    }
    #endregion
}
