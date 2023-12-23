using UnityEngine;
using UnityEditor;

namespace DustEngine.DustEditor
{
    [CustomEditor(typeof(DuTwistDeformer))]
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    public class DuTwistDeformerEditor : DuDeformerEditor
    {
        private DuProperty m_DeformMode;
        private DuProperty m_Size;
        private DuProperty m_Angle;
        private DuProperty m_Direction;

        //--------------------------------------------------------------------------------------------------------------

        static DuTwistDeformerEditor()
        {
            DuDeformersPopupButtons.AddDeformer(typeof(DuTwistDeformer), "Twist");
        }

        [MenuItem("Dust/Deformers/Twist")]
        public static void AddComponent()
        {
            AddDeformerComponentByType(typeof(DuTwistDeformer));
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            m_DeformMode = FindProperty("m_DeformMode", "Deform Mode");
            m_Size = FindProperty("m_Size", "Size");
            m_Angle = FindProperty("m_Angle", "Angle");
            m_Direction = FindProperty("m_Direction", "Direction");
        }

        public override void OnInspectorGUI()
        {
            InspectorInitStates();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            if (DustGUI.FoldoutBegin("Parameters", "DuTwistDeformer.Params"))
            {
                PropertyField(m_Size);
                PropertyExtendedSlider(m_Angle, -360f, 360f, 1f);
                PropertyField(m_DeformMode);
                PropertyField(m_Direction);
                Space();
            }
            DustGUI.FoldoutEnd();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            OnInspectorGUI_FieldsMap();
            OnInspectorGUI_GizmoBlock();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            if (m_Size.isChanged)
                m_Size.valVector3 = DuTwistDeformer.Normalizer.Size(m_Size.valVector3);

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            InspectorCommitUpdates();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // Require forced redraw scene view

            DustGUI.ForcedRedrawSceneView();
        }
    }
}
