using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Addons
{
    public class WeaponBarrelRecoil : MonoBehaviour
    {
        [Tooltip("Curve describing the Barrel Movement")]
        public AnimationCurve Movement;

        [Tooltip("Barrel Maximum Movement Amplitude on the 3 Axis")]
        public Vector3 Amplitude;

        [Range(0f, 1f), Tooltip("Duration of the animation")]
        public float Duration = 0.5f;

        public WeaponController WeaponController;
        Vector3 m_InitialLocalPosition;
        float m_StartTimestamp = 0f;
        float m_ChargeRatioOnShoot = 0f;

        void Awake()
        {
            WeaponController.OnShootProcessed += StartRecoilAnimation;
            m_InitialLocalPosition = transform.localPosition;
        }

        void StartRecoilAnimation()
        {
            m_ChargeRatioOnShoot = WeaponController.CurrentCharge;
            m_StartTimestamp = Time.time;
        }

        void Update()
        {
            if (m_StartTimestamp > 0f && m_StartTimestamp + Duration > Time.time && Duration > 0f)
            {
                float ratio = 1f - ((m_StartTimestamp + Duration) - Time.time) / Duration;
                float displacement = Movement.Evaluate(ratio);
                transform.localPosition = m_InitialLocalPosition
                                          + (Amplitude * displacement * m_ChargeRatioOnShoot);
            }
            else
            {
                transform.localPosition = m_InitialLocalPosition;
            }
        }

        void OnDestroy()
        {
            WeaponController.OnShootProcessed -= StartRecoilAnimation;
        }
    }
}