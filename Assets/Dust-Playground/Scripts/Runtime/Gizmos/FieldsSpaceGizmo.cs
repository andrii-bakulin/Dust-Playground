using UnityEngine;
using UnityEditor;

namespace DustEngine
{
    [AddComponentMenu("Dust-Playground/Gizmos/Fields Space Gizmo")]
    [ExecuteInEditMode]
    public class FieldsSpaceGizmo : AbstractGizmo
    {
        [SerializeField]
        private FieldsSpace m_FieldsSpace;
        public FieldsSpace fieldsSpace
        {
            get => m_FieldsSpace;
            set => m_FieldsSpace = value;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        [SerializeField]
        private Vector3Int m_GridCount = new Vector3Int(9, 1, 9);
        public Vector3Int gridCount
        {
            get => m_GridCount;
            set => m_GridCount = NormalizeGridCount(value);
        }

        [SerializeField]
        private Vector3 m_GridStep = Vector3.one;
        public Vector3 gridStep
        {
            get => m_GridStep;
            set => m_GridStep = value;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        [SerializeField]
        private bool m_PowerVisible = false;
        public bool powerVisible
        {
            get => m_PowerVisible;
            set => m_PowerVisible = value;
        }

        [SerializeField]
        private float m_PowerSize = 1f;
        public float powerSize
        {
            get => m_PowerSize;
            set => m_PowerSize = NormalizeSize(value);
        }

        [SerializeField]
        private bool m_PowerDotsVisible = false;
        public bool powerDotsVisible
        {
            get => m_PowerDotsVisible;
            set => m_PowerDotsVisible = value;
        }

        [SerializeField]
        private float m_PowerDotsSize = 0.4f;
        public float powerDotsSize
        {
            get => m_PowerDotsSize;
            set => m_PowerDotsSize = NormalizeSize(value);
        }

        [SerializeField]
        private bool m_PowerImpactOnDotsSize = false;
        public bool powerImpactOnDotsSize
        {
            get => m_PowerImpactOnDotsSize;
            set => m_PowerImpactOnDotsSize = value;
        }

        [SerializeField]
        private Gradient m_PowerDotsColor = DuGradient.CreateBlackToRed();
        public Gradient powerDotsColor
        {
            get => m_PowerDotsColor;
            set => m_PowerDotsColor = value;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        [SerializeField]
        private bool m_ColorVisible = true;
        public bool colorVisible
        {
            get => m_ColorVisible;
            set => m_ColorVisible = value;
        }

        [SerializeField]
        private float m_ColorSize = 1f;
        public float colorSize
        {
            get => m_ColorSize;
            set => m_ColorSize = NormalizeSize(value);
        }

        [SerializeField]
        private bool m_PowerImpactOnColorSize = false;
        public bool powerImpactOnColorSize
        {
            get => m_PowerImpactOnColorSize;
            set => m_PowerImpactOnColorSize = value;
        }

        [SerializeField]
        private bool m_ColorAllowTransparent = true;
        public bool colorAllowTransparent
        {
            get => m_ColorAllowTransparent;
            set => m_ColorAllowTransparent = value;
        }

        //--------------------------------------------------------------------------------------------------------------

        public override string GizmoName()
        {
            return "Fields Space";
        }

#if UNITY_EDITOR
        protected override void DrawGizmos()
        {
            if (Dust.IsNull(fieldsSpace))
                return;

            bool showPower = powerVisible && fieldsSpace.fieldsMap.calculatePower;
            bool showColor = colorVisible && fieldsSpace.fieldsMap.calculateColor;

            if (!showPower && !showColor)
                return;

            GUIStyle style = new GUIStyle("Label");
            style.fontSize = Mathf.RoundToInt(style.fontSize * powerSize);

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            Vector3 zeroPoint;
            zeroPoint.x = -(gridCount.x - 1) / 2f * gridStep.x;
            zeroPoint.y = -(gridCount.y - 1) / 2f * gridStep.y;
            zeroPoint.z = -(gridCount.z - 1) / 2f * gridStep.z;

            float offset = 0f;
            float deltaOffset = 1f / Mathf.Max(1, gridCount.x * gridCount.y * gridCount.z);

            for (int z = 0; z < gridCount.z; z++)
            for (int y = 0; y < gridCount.y; y++)
            for (int x = 0; x < gridCount.x; x++)
            {
                var position = zeroPoint + new Vector3(gridStep.x * x, gridStep.y * y, gridStep.z * z);

                Vector3 worldPosition = transform.TransformPoint(position);

                Color fieldColor;
                float fieldPower = fieldsSpace.GetPowerAndColor(worldPosition, offset, out fieldColor);

                if (showColor)
                {
                    if (!colorAllowTransparent)
                        fieldColor = ColorBlend.AlphaBlend(Color.black, fieldColor);

                    float dotSize = 0.1f * colorSize;
                    if (powerImpactOnColorSize)
                        dotSize *= fieldPower;

                    Handles.color = fieldColor;
                    Handles.DotHandleCap(0, worldPosition, transform.rotation, dotSize, EventType.Repaint);
                }

                if (showPower)
                {
                    Handles.Label(worldPosition, fieldPower.ToString("F2"), style);

                    if (powerDotsVisible)
                    {
                        if (Dust.IsNotNull(powerDotsColor))
                            Handles.color = powerDotsColor.Evaluate(fieldPower);
                        else
                            Handles.color = Color.Lerp(Color.black, Color.white, fieldPower);

                        float dotSize = 0.1f * powerDotsSize;
                        if (powerImpactOnDotsSize)
                            dotSize *= fieldPower;

                        Handles.DotHandleCap(0, worldPosition, transform.rotation, dotSize, EventType.Repaint);
                    }
                }

                offset += deltaOffset;
            }
        }
#endif

        //--------------------------------------------------------------------------------------------------------------
        // Normalizer

        public static float NormalizeSize(float value)
        {
            return Mathf.Clamp(value, 0.1f, float.MaxValue);
        }

        public static Vector3Int NormalizeGridCount(Vector3Int value)
        {
            return DuVector3Int.Clamp(value, Vector3Int.one, Vector3Int.one * 1000);
        }
    }
}
