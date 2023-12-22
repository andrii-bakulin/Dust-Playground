using UnityEngine;
using UnityEditor;

namespace DustEngine.DustEditor
{
    [CustomEditor(typeof(ParallaxController))]
    [CanEditMultipleObjects]
    public class ParallaxControllerEditor : DuEditor
    {
        private DuProperty m_ParallaxControl;
        private DuProperty m_Offset;
        private DuProperty m_TimeScale;
        private DuProperty m_Freeze;

        private DuProperty m_UpdateMode;

        //--------------------------------------------------------------------------------------------------------------

        [MenuItem("Dust-Playground/Instances/Parallax Controller")]
        public static void AddComponentToSelectedObjects()
        {
            AddComponentToSelectedOrNewObject("Parallax Controller", typeof(ParallaxController));
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            m_ParallaxControl = FindProperty("m_ParallaxControl", "Parallax Control");
            m_Offset = FindProperty("m_Offset", "Offset");
            m_TimeScale = FindProperty("m_TimeScale", "Time Scale");
            m_Freeze = FindProperty("m_Freeze", "Freeze");

            m_UpdateMode = FindProperty("m_UpdateMode", "Update Mode");
        }

        public override void OnInspectorGUI()
        {
            InspectorInitStates();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            DustGUI.FoldoutBegin("Control");
            {
                PropertyField(m_ParallaxControl);

                switch ((ParallaxController.ParallaxControl) m_ParallaxControl.valInt)
                {
                    case ParallaxController.ParallaxControl.Manual:
                        PropertyExtendedSlider(m_Offset, 0f, 10f, 0.01f);
                        break;

                    case ParallaxController.ParallaxControl.Time:
                        PropertyExtendedSlider(m_Offset, 0f, 10f, 0.01f);
                        PropertyExtendedSlider(m_TimeScale, -10f, 10f, 0.01f);
                        PropertyField(m_Freeze);
                        break;

                    default:
                        break;
                }

                Space();
            }
            DustGUI.FoldoutEnd();

            DustGUI.FoldoutBegin("Others");
            {
                PropertyField(m_UpdateMode);
                Space();
            }
            DustGUI.FoldoutEnd();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            InspectorCommitUpdates();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            if (m_Offset.isChanged)
            {
                var parallaxChildren = FindObjectsOfType<Parallax>();

                foreach (var subTarget in targets)
                {
                    var origin = subTarget as ParallaxController;
                    origin.UpdateState(0f);

                    foreach (var parallaxChild in parallaxChildren)
                    {
                        if (parallaxChild.parallaxController != origin)
                            continue;

                        parallaxChild.UpdateState(0f);
                    }
                }
            }
        }
    }
}
