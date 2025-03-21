using UnityEngine;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// Shake camera on player dies
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; set; }
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeAmount = 0.7f;
        [SerializeField] private float decreaseFactor = 1.0f;
        [SerializeField] private float currentShakeDuration = 0f;

        private void OnEnable()
        {
            EventManager.OnGameOver += TriggerShake;
        }
        private void OnDisable()
        {
            EventManager.OnGameOver -= TriggerShake;
        }
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (currentShakeDuration > 0)
            {
                transform.position = transform.position + Random.insideUnitSphere * shakeAmount;

                currentShakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                currentShakeDuration = 0f;
            }
        }

        public void TriggerShake()
        {
            currentShakeDuration = shakeDuration;
        }
    }
}
