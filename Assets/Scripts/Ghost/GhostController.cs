using System.Collections.Generic;
using UnityEngine;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// Ghost controller class which mimics the player movements
    /// </summary>
    public class GhostController : MonoBehaviour
    {
        [SerializeField] private float baseJumpHeight = 2.0f;
        [SerializeField] private float baseJumpDuration = 0.5f;
        [SerializeField] private float fallMultiplier = 1.3f;
        [SerializeField] private float GameManagerInstance = 0.2f;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private PlatformController platformController;
        [SerializeField] private float movementInterpolationSpeed = 10f; // Added for smooth movement

        public bool isJumping;
        private float jumpStartTime;
        private float initialY;
        private float jumpDuration;
        private float jumpHeight;
        private float currentSpeed;
        public float minSpeed = 5f;
        public float maxSpeed = 15f;
        public float acceleration = 0.2f;
        private Queue<GhostAction> actionQueue = new Queue<GhostAction>();

        // Track target position for smooth movement
        private Vector3 targetPosition;
        private bool hasTargetPosition = false;

        private void OnEnable()
        {
            EventManager.OnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            EventManager.OnGameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            currentSpeed = minSpeed;
            actionQueue.Clear();
            hasTargetPosition = false;
        }

        private void Start()
        {
            initialY = transform.position.y;
            currentSpeed = minSpeed;
            targetPosition = transform.position;
        }

        public void Init(float _baseJumpHeight, float _baseJumpDuration, float _fallMultiplier, float _currentSpeed)
        {
            baseJumpHeight = _baseJumpHeight;
            baseJumpDuration = _baseJumpDuration;
            fallMultiplier = _fallMultiplier;
            currentSpeed = _currentSpeed;
        }

        void Update()
        {
            ProcessActionQueue();

            // Continue the jump if it's active
            if (isJumping)
            {
                ContinueJump();
            }

            // Platform reset logic
            if (transform.position.z > GameManager.Instance.dist)
            {
                float offset = transform.position.z - 0f;
                platformController.ResetWorld(offset);
                Vector3 resetPosition = new Vector3(transform.position.x, transform.position.y, 0f);
                transform.position = resetPosition;
                return;
            }
            else
            {
                // Smoothly move toward target position
                if (hasTargetPosition)
                {
                    Vector3 currentPos = transform.position;
                    Vector3 smoothPosition = new Vector3(
                        currentPos.x,
                        currentPos.y,
                        Mathf.Lerp(currentPos.z, targetPosition.z, Time.deltaTime * movementInterpolationSpeed)
                    );
                    transform.position = smoothPosition;
                }
            }
        }

        public void QueueResetPosition(Vector3 newPosition)
        {
            GhostAction action = new GhostAction
            {
                Type = ActionType.ResetPosition,
                TargetPosition = newPosition,
                ExecutionTime = Time.time + GameManager.Instance.syncDelay
            };

            actionQueue.Enqueue(action);
        }

        private void ProcessActionQueue()
        {
            if (actionQueue == null)
                return;

            while (actionQueue.Count > 0 && Time.time >= actionQueue.Peek().ExecutionTime)
            {
                GhostAction action = actionQueue.Dequeue();

                switch (action.Type)
                {
                    case ActionType.Move:
                        targetPosition = new Vector3(
                            transform.position.x,
                            transform.position.y,
                            action.TargetPosition.z
                        );
                        hasTargetPosition = true;
                        break;

                    case ActionType.Jump:
                        // Adjust jump delay to ensure it starts at the correct time
                        if (!isJumping && Time.time >= action.ExecutionTime)
                        {
                            StartJump(action.JumpStartTime, action.JumpDuration);
                        }
                        break;

                    case ActionType.CollectOrb:
                        if (particle != null)
                        {
                            particle.Play();
                        }
                        break;
                }
            }
        }

        void StartJump(float startTime, float duration)
        {
            // Ensure ghost starts jump from correct Y position
            if (transform.position.y != initialY)
            {
                transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
            }

            isJumping = true;
            jumpStartTime = Time.time;  // Use current time to start the jump
            jumpDuration = duration;
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
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
        }

        public void QueueJumpAction(float playerJumpStartTime, float playerJumpDuration)
        {
            GhostAction action = new GhostAction
            {
                Type = ActionType.Jump,
                JumpStartTime = playerJumpStartTime + GameManager.Instance.syncDelay,  // Apply delay correctly
                JumpDuration = playerJumpDuration,
                ExecutionTime = Time.time + GameManager.Instance.syncDelay
            };

            actionQueue.Enqueue(action);
        }

        public void QueueMovementPosition(Vector3 playerPosition)
        {
            GhostAction action = new GhostAction
            {
                Type = ActionType.Move,
                TargetPosition = playerPosition,
                ExecutionTime = Time.time + GameManager.Instance.syncDelay
            };

            actionQueue.Enqueue(action);
        }

        public void QueueCollectOrbAction()
        {
            GhostAction action = new GhostAction
            {
                Type = ActionType.CollectOrb,
                ExecutionTime = Time.time + GameManager.Instance.syncDelay
            };

            actionQueue.Enqueue(action);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Orb"))
            {
                particle.transform.position = other.transform.position;
                particle.Play();
                other.gameObject.SetActive(false);
            }
        }

        public enum ActionType
        {
            Move,
            Jump,
            CollectOrb,
            ResetPosition
        }

        private struct GhostAction
        {
            public ActionType Type;
            public Vector3 TargetPosition;
            public float MovementDistance;
            public float ExecutionTime;
            public float JumpStartTime;
            public float JumpDuration;
        }
    }
}
