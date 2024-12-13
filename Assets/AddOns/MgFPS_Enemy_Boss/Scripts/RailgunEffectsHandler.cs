using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Addons
{
    [RequireComponent(typeof(WeaponController))]
    public class RailgunEffectsHandler : MonoBehaviour
    {
        [Header("Visual")] [Tooltip("Root transform where the particles will be instantiated")]
        public Transform particlesRootTransform;

        [Tooltip("Outer particles instantiated when charging")]
        public ParticleSystem railgunChargePrefab;

        [Tooltip("Inner particles instantiated when charging")]
        public ParticleSystem railgunFocusPointPrefab;

        [Header("Sound")] [Tooltip("Audio clip for charge SFX")]
        public AudioClip chargeSound;

        [Tooltip("Factor by which the charge SFX duration is multiplied")]
        public float chargeSoundDurationFactor = 0.9f;

        bool m_WasWeaponCharging;
        WeaponController m_WeaponController;
        ParticleSystem m_RailgunChargeInstance;
        ParticleSystem m_RailgunFocusPointInstance;
        AudioSource m_AudioSource;

        void Start()
        {
            m_WeaponController = GetComponent<WeaponController>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, RailgunEffectsHandler>(m_WeaponController,
                this, gameObject);

            // The charge effect needs it's own AudioSources, since it will play on top of the other gun sounds
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = chargeSound;
            m_AudioSource.playOnAwake = false;
            m_AudioSource.outputAudioMixerGroup =
                AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.EnemyAttack);

            m_WasWeaponCharging = false;
        }

        void Update()
        {
            // Check if the weapon charging state changed during this frame
            if (m_WeaponController.IsCharging != m_WasWeaponCharging)
            {
                // Check if the weapon started charging during this frame
                if (m_WeaponController.IsCharging)
                {
                    m_RailgunChargeInstance = Instantiate(railgunChargePrefab, particlesRootTransform);
                    m_RailgunFocusPointInstance = Instantiate(railgunFocusPointPrefab, particlesRootTransform);

                    var particleModule = m_RailgunChargeInstance.main;
                    particleModule.startLifetimeMultiplier = m_WeaponController.MaxChargeDuration;
                    particleModule.duration = m_WeaponController.MaxChargeDuration;

                    particleModule = m_RailgunFocusPointInstance.main;
                    particleModule.startLifetimeMultiplier = m_WeaponController.MaxChargeDuration;

                    m_RailgunChargeInstance.Play();
                    m_RailgunFocusPointInstance.Play();

                    m_AudioSource.pitch = m_AudioSource.clip.length / m_WeaponController.MaxChargeDuration *
                                          chargeSoundDurationFactor;
                    m_AudioSource.Play();
                }
                else
                {
                    Destroy(m_RailgunChargeInstance.gameObject);
                    Destroy(m_RailgunFocusPointInstance.gameObject);
                    m_AudioSource.Stop();
                }
            }

            m_WasWeaponCharging = m_WeaponController.IsCharging;
        }
    }
}