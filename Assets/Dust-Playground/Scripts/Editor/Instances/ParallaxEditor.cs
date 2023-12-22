using UnityEngine;
using UnityEditor;

namespace DustEngine.DustEditor
{
    [CustomEditor(typeof(Parallax))]
    [CanEditMultipleObjects]
    public class ParallaxEditor : DuEditor
    {
        private DuProperty m_ParallaxControl;
        private DuProperty m_Offset;
        private DuProperty m_ParallaxController;
        private DuProperty m_TimeScale;
        private DuProperty m_Freeze;
        private DuProperty m_Speed;

        private DuProperty m_TileObject;
        private DuProperty m_TileLength;
        private DuProperty m_TilesCount;
        private DuProperty m_TileOffset;

        private DuProperty m_UpdateMode;
        private DuProperty m_GizmoVisibility;

        //--------------------------------------------------------------------------------------------------------------

        [MenuItem("Dust-Playground/Instances/Parallax")]
        public static void AddComponentToSelected()
        {
            AddComponentToSelectedOrNewObject("Parallax", typeof(Parallax));
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            m_ParallaxControl = FindProperty("m_ParallaxControl", "Parallax Control");
            m_Offset = FindProperty("m_Offset", "Offset");
            m_ParallaxController = FindProperty("m_ParallaxController", "Parallax Controller");
            m_TimeScale = FindProperty("m_TimeScale", "Time Scale");
            m_Speed = FindProperty("m_Speed", "Speed");
            m_Freeze = FindProperty("m_Freeze", "Freeze");

            m_TileObject = FindProperty("m_TileObject", "Tile Object");
            m_TileLength = FindProperty("m_TileLength", "Tile Length");
            m_TilesCount = FindProperty("m_TilesCount", "Tiles Count");
            m_TileOffset = FindProperty("m_TileOffset", "Tile Offset");

            m_UpdateMode = FindProperty("m_UpdateMode", "Update Mode");
            m_GizmoVisibility = FindProperty("m_GizmoVisibility", "Gizmo Visibility");
        }

        public override void OnInspectorGUI()
        {
            InspectorInitStates();

            bool isRequireRebuildTiles = DustGUI.IsUndoRedoPerformed();
            bool isRequireResetupTiles = DustGUI.IsUndoRedoPerformed();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            if (DustGUI.FoldoutBegin("Control", "Parallax.Control"))
            {
                PropertyField(m_ParallaxControl);

                switch ((Parallax.ParallaxControl) m_ParallaxControl.valInt)
                {
                    case Parallax.ParallaxControl.Manual:
                        PropertyExtendedSlider(m_Offset, -10f, +10f, 0.01f);
                        break;

                    case Parallax.ParallaxControl.Time:
                        PropertyExtendedSlider(m_Offset, -10f, +10f, 0.01f);
                        PropertyExtendedSlider(m_TimeScale, -10f, 10f, 0.01f);
                        break;

                    case Parallax.ParallaxControl.ParallaxController:
                        PropertyField(m_ParallaxController);
                        break;

                    default:
                        break;
                }

                Space();

                PropertyExtendedSlider(m_Speed, 0f, 10f, 0.01f);
                PropertyField(m_Freeze);
            }
            DustGUI.FoldoutEnd();

            if (DustGUI.FoldoutBegin("Parameters", "Parallax.Parameters"))
            {
                PropertyField(m_TileObject);
                PropertyExtendedSlider(m_TileLength, 0f, 25f, 0.01f);
                PropertyExtendedIntSlider(m_TilesCount, 2, 16, 1, 2);
                PropertySlider(m_TileOffset, 0f, 1f);
            }
            DustGUI.FoldoutEnd();

            if (DustGUI.FoldoutBegin("Others", "Parallax.Others"))
            {
                if ((Parallax.ParallaxControl) m_ParallaxControl.valInt == Parallax.ParallaxControl.ParallaxController)
                {
                    DustGUI.StaticTextField(m_UpdateMode.title, "Inherited from Parallax Controller");
                }
                else
                {
                    PropertyField(m_UpdateMode);
                }
                PropertyField(m_GizmoVisibility);
            }
            DustGUI.FoldoutEnd();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // Validate & Normalize Data

            if (m_TileOffset.isChanged)
                m_TileOffset.valFloat = Parallax.NormalizeTileOffset(m_TileOffset.valFloat);

            if (m_TilesCount.isChanged)
                m_TilesCount.valInt = Parallax.NormalizeTilesCount(m_TilesCount.valInt);

            InspectorCommitUpdates();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            isRequireRebuildTiles |= m_TileObject.isChanged;
            isRequireRebuildTiles |= m_TilesCount.isChanged;

            isRequireResetupTiles |= m_ParallaxControl.isChanged;
            isRequireResetupTiles |= m_Offset.isChanged;
            isRequireResetupTiles |= m_ParallaxController.isChanged;
            isRequireResetupTiles |= m_TileLength.isChanged;
            isRequireResetupTiles |= m_TileOffset.isChanged;

            foreach (var subTarget in targets)
            {
                var origin = subTarget as Parallax;

                if (isRequireRebuildTiles)
                {
                    origin.RebuildTiles();
                }
                else if (isRequireResetupTiles)
                {
                    origin.UpdateState(0f);
                }
            }
        }
    }
}
