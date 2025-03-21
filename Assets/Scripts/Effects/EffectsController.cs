using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// manages all in game visual effects including the post processing
    /// </summary>
    public class EffectsController : Singletone<EffectsController>
    {
        [SerializeField] private PostProcessVolume postProcessVolume;  // The post-processing volume
        [SerializeField] private float minSpeed = 5f;   // Minimum speed for motion blur to start
        [SerializeField] private float maxSpeed = 15f;  // Maximum speed for maximum motion blur
        [SerializeField] private float maxShutterAngle = 270f;  // Maximum shutter angle for blur intensity
        private ChromaticAberration chromaticAberration;
        private MotionBlur motionBlur;
        public bool isCrashing = false;
        public float crashEffectDuration = 1.0f;
        public float crashEffectIntensity = 0.5f;
        private void OnEnable()
        {
            EventManager.OnGameOver += TriggerCrashEffect;
        }
        private void OnDisable()
        {
            EventManager.OnGameOver -= TriggerCrashEffect;
        }
        protected override void Awake()
        {
            base.Awake();
        }
        void Start()
        {
            // Get the Motion Blur effect from the Post-Processing Volume
            postProcessVolume.profile.TryGetSettings(out motionBlur);
            // Get the chromatic aberration from the post-processing volume
            postProcessVolume.profile.TryGetSettings(out chromaticAberration);
            // Get a reference to the PlatformController to access the player's speed
        }

        void Update()
        {
            // Get the current player speed from the PlatformController
            float currentSpeed = GameManager.Instance.currentSpeed;

            // Normalize the speed to a value between 0 and 1
            float speedFactor = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);

            // Adjust the motion blur intensity based on the speed factor
            motionBlur.shutterAngle.value = Mathf.Lerp(0f, maxShutterAngle, speedFactor);

            if (isCrashing)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, crashEffectIntensity, Time.deltaTime);
            }
            else
            {
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0, Time.deltaTime);
            }

            if (rippleMaterial != null && rippleCenterObject != null)
            {
                Vector3 screenPos = Camera.main.WorldToViewportPoint(rippleCenterObject.position);
                rippleMaterial.SetVector("_RippleCenter", new Vector4(screenPos.x, screenPos.y, 0, 0));
                rippleMaterial.SetFloat("_RippleStrength", rippleStrength);
                rippleMaterial.SetFloat("_RippleTime", rippleTime);

                rippleTime += Time.deltaTime * rippleSpeed;
            }
        }

        public void TriggerCrashEffect()
        {
            isCrashing = true;
            Invoke("StopCrashEffect", crashEffectDuration);
            TriggerRipple();
        }

        void StopCrashEffect()
        {
            isCrashing = false;
        }

        public Material rippleMaterial;
        public Transform rippleCenterObject;
        public float rippleSpeed = 1.0f;
        public float rippleStrength = 0.05f;

        private float rippleTime = 0.0f;

        public void TriggerRipple()
        {
            rippleTime = 0.0f;
        }
    }
}