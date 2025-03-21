using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float baseJumpHeight = 2.0f;
    [SerializeField] private float baseJumpDuration = 0.5f;
    [SerializeField] private float fallMultiplier = 1.3f;
    [SerializeField] private float networkDelay = 0.2f;
    [SerializeField] private ParticleSystem particle;


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
        for(int i = 0; i < actionQueue.Count; i++)
        {
            actionQueue.Dequeue();
        }
    }

    private void Start()
    {
        initialY = transform.position.y;
        currentSpeed = minSpeed;  // Initialize ghost's speed to minimum speed
    }

    public void Init(float _baseJumpHeight, float _baseJumpDuration, float _fallMultiplier, float _currentSpeed)
    {
        baseJumpHeight = _baseJumpHeight;
        baseJumpDuration = _baseJumpDuration;
        fallMultiplier = _fallMultiplier;
        currentSpeed = _currentSpeed;  // Ensure ghost's speed is set
    }


    void Update()
    {
        ProcessActionQueue();

        // Continue the jump if it's active
        if (isJumping)
        {
            ContinueJump();
        }
    }


    public void QueueResetPosition(Vector3 newPosition)
    {
        GhostAction action = new GhostAction
        {
            Type = ActionType.ResetPosition,
            TargetPosition = newPosition,  // Target position to reset to
            ExecutionTime = Time.time + networkDelay
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
                    transform.position = new Vector3(transform.position.x, transform.position.y, action.TargetPosition.z);
                    break;

                case ActionType.Jump:
                    if (!isJumping)
                    {
                        StartJump(action.JumpStartTime, action.JumpDuration);  // Pass the correct start time and duration
                    }
                    break;

                case ActionType.CollectOrb:
                    if (particle != null)
                    {
                        particle.Play();
                    }
                    break;

                case ActionType.ResetPosition:
                    transform.position = new Vector3(transform.position.x, transform.position.y, action.TargetPosition.z);
                    break;
            }
        }
    }



    // Update the StartJump method to use the correct start time and duration
    void StartJump(float startTime, float duration)
    {
        isJumping = true;
        jumpStartTime = startTime;
        jumpDuration = duration;  // Use the passed jump duration from the player

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

    public void QueueJumpAction(float playerJumpStartTime, float playerJumpDuration)
    {
        GhostAction action = new GhostAction
        {
            Type = ActionType.Jump,
            JumpStartTime = playerJumpStartTime + networkDelay,  // Apply delay to the jump start time
            JumpDuration = playerJumpDuration,  // Keep the same jump duration as the player
            ExecutionTime = Time.time + networkDelay
        };

        actionQueue.Enqueue(action);
    }

    // Queue the player's position for the ghost to follow
    public void QueueMovementPosition(Vector3 playerPosition)
    {
        GhostAction action = new GhostAction
        {
            Type = ActionType.Move,
            TargetPosition = playerPosition,  // Queue the player's position
            ExecutionTime = Time.time + networkDelay // Apply delay
        };

        actionQueue.Enqueue(action);
    }



    public void QueueCollectOrbAction()
    {
        GhostAction action = new GhostAction
        {
            Type = ActionType.CollectOrb,
            ExecutionTime = Time.time + networkDelay
        };

        actionQueue.Enqueue(action);
    }

    public enum ActionType
    {
        Move,
        Jump,
        CollectOrb,
        ResetPosition  // Add reset action
    }


    private struct GhostAction
    {
        public ActionType Type;
        public Vector3 TargetPosition;
        public float MovementDistance;
        public float ExecutionTime;
        public float JumpStartTime;  // New field for the jump start time
        public float JumpDuration;   // New field for the jump duration
    }

}
