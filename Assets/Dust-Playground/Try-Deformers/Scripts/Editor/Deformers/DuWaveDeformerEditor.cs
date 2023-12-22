using UnityEngine;
using UnityEditor;

namespace DustEngine.DustEditor
{
    [CustomEditor(typeof(DuWaveDeformer))]
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    public class DuWaveDeformerEditor : DuDeformerEditor
    {
        private DuProperty m_Amplitude;
        private DuProperty m_Frequency;
        private DuProperty m_LinearFalloff;
        private DuProperty m_Offset;
        private DuProperty m_AnimationSpeed;
        private DuProperty m_Direction;

        private DuProperty m_GizmoSize;
        private DuProperty m_GizmoQuality;
        private DuProperty m_GizmoAnimated;

        //--------------------------------------------------------------------------------------------------------------

        static DuWaveDeformerEditor()
        {
            DuDeformersPopupButtons.AddDeformer(typeof(DuWaveDeformer), "Wave");
        }

        [MenuItem("Dust/Deformers/Wave")]
        public static void AddComponent()
        {
            AddDeformerComponentByType(typeof(DuWaveDeformer));
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            m_Amplitude = FindProperty("m_Amplitude", "Amplitude");
            m_Frequency = FindProperty("m_Frequency", "Frequency");
            m_LinearFalloff = FindProperty("m_LinearFalloff", "Linear Falloff");
            m_Offset = FindProperty("m_Offset", "Offset");
            m_AnimationSpeed = FindProperty("m_AnimationSpeed", "Animation Speed");
            m_Direction = FindProperty("m_Direction", "Direction");

            m_GizmoSize = FindProperty("m_GizmoSize", "Size");
            m_GizmoQuality = FindProperty("m_GizmoQuality", "Quality");
            m_GizmoAnimated = FindProperty("m_GizmoAnimated", "Animate in Editor");
        }

        public override void OnInspectorGUI()
        {
            InspectorInitStates();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            if (DustGUI.FoldoutBegin("Parameters", "DuWaveDeformer.Params"))
            {
                PropertyExtendedSlider(m_Amplitude, 0f, 10f, 0.01f);
                PropertyExtendedSlider(m_Frequency, 0f, 10f, 0.01f);
                PropertyExtendedSlider(m_LinearFalloff, 0f, 10f, 0.01f);
                Space();

                PropertyExtendedSlider(m_Offset, 0f, 1f, 0.01f);
                PropertyExtendedSlider(m_AnimationSpeed, -2f, +2f, 0.01f);
                PropertyField(m_Direction);
                Space();
            }
            DustGUI.FoldoutEnd();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            OnInspectorGUI_FieldsMap();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // @ignore: OnInspectorGUI_GizmoBlock();

            if (DustGUI.FoldoutBegin("Gizmo", "DuDeformer.Gizmo"))
            {
                PropertyExtendedSlider(m_GizmoSize, 0.1f, 10f, 0.1f);
                PropertyField(m_GizmoQuality);
                PropertyField(m_GizmoVisibility);
                PropertyFieldOrLock(m_GizmoAnimated, DuMath.IsZero(m_AnimationSpeed.valFloat));
                Space();
            }
            DustGUI.FoldoutEnd();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            InspectorCommitUpdates();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // Require forced redraw scene view

            DustGUI.ForcedRedrawSceneView();
        }
    }
}
