using UnityEngine;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// manages forward movement and give references of speed and pos data to the Ghost
    /// </summary>
    public class CubeController : MonoBehaviour
    {
        [SerializeField] private float forwardSpeed = 5f;  // Player forward speed
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private GhostController ghostController;
        [SerializeField] private ParticleSystem burstParticle;
        [SerializeField] private PlatformController platformController;

        public float currentSpeed;
        public float minSpeed = 5f;
        public float maxSpeed = 15f;
        public float acceleration = 0.2f;

        private void Start()
        {
            currentSpeed = minSpeed;
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
                platformController.ResetWorld(offset);
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
                //ghostController.QueueResetPosition(resetPosition);
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
    }
}
