using UnityEngine;

namespace DustEngine
{
    [AddComponentMenu("Dust-Playground/Instances/Parallax Controller")]
    public class ParallaxController : DuMonoBehaviour
    {
        public enum ParallaxControl
        {
            Manual = 0,
            Time = 1,
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        [SerializeField]
        private ParallaxControl m_ParallaxControl = ParallaxControl.Manual;
        public ParallaxControl parallaxControl
        {
            get => m_ParallaxControl;
            set => m_ParallaxControl = value;
        }

        [SerializeField]
        private float m_Offset = 0f;
        public float offset
        {
            get => m_Offset;
            set => m_Offset = value;
        }

        [SerializeField]
        private float m_TimeScale = 1f;
        public float timeScale
        {
            get => m_TimeScale;
            set => m_TimeScale = value;
        }

        [SerializeField]
        private bool m_Freeze = false;
        public bool freeze
        {
            get => m_Freeze;
            set => m_Freeze = value;
        }

        [SerializeField]
        private UpdateMode m_UpdateMode = UpdateMode.Update;
        public UpdateMode updateMode
        {
            get => m_UpdateMode;
            set => m_UpdateMode = value;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private float m_OffsetDynamic;

        //--------------------------------------------------------------------------------------------------------------

        void Update()
        {
            if (updateMode != UpdateMode.Update)
                return;

            UpdateState(Time.deltaTime);
        }

        void LateUpdate()
        {
            if (updateMode != UpdateMode.LateUpdate)
                return;

            UpdateState(Time.deltaTime);
        }

        void FixedUpdate()
        {
            if (updateMode != UpdateMode.FixedUpdate)
                return;

            UpdateState(Time.fixedDeltaTime);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public void UpdateState(float deltaTime)
        {
            if (freeze)
                return;

            switch (parallaxControl)
            {
                case ParallaxControl.Manual:
                    // Nothing need to do
                    break;

                case ParallaxControl.Time:
                    m_OffsetDynamic += deltaTime * timeScale;
                    break;

                default:
                    break;
            }
        }

        public float GetGlobalOffset()
        {
            return offset + m_OffsetDynamic;
        }
    }
}
