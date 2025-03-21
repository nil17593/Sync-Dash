using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float baseJumpHeight = 2.0f;
    [SerializeField] private float baseJumpDuration = 0.5f;
    [SerializeField] private ParticleSystem burstParticle;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GhostController ghostController;
    [SerializeField] private CubeController cubeController;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float acceleration = 0.2f;

    public bool isJumping;
    private float jumpStartTime;
    private float initialY;
    private float jumpDuration;
    private float jumpHeight;
    private float fallMultiplier = 1.3f;

    private void Start()
    {
        initialY = transform.position.y;
    }
    private void Update()
    {
        if (GameManager.Instance.GameOver)
            return;
        if (ISGrounded()  && Input.GetMouseButtonDown(0) && !isJumping)
        {
            StartJump();

            if (ghostController != null)
            {
                ghostController.QueueJumpAction(jumpStartTime, jumpDuration);
            }
        }

        if (isJumping)
        {
            ContinueJump();
        }

    }


    public void StartJump()
    {
        // Initialize jump variables
        isJumping = true;
        jumpStartTime = Time.time;

        float speedRatio = cubeController.GetCurrentSpeed() / minSpeed;
        jumpDuration = baseJumpDuration / speedRatio;
        jumpHeight = baseJumpHeight;
    }

    bool ISGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer); ;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Orb"))
        {
            burstParticle.Play();
            burstParticle.transform.position = other.transform.position;
            ScoreManager.Instance.AddCollectibleScore(5);
            other.gameObject.SetActive(false);
        }
        if (other.gameObject.GetComponent<Obstacle>() != null)
        {
            GameManager.Instance.GameOver = true;
            EventManager.TriggerGameOverEvent();
        }
    }
}


