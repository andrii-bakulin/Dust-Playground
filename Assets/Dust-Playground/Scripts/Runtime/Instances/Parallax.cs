using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DustEngine
{
    [AddComponentMenu("Dust-Playground/Instances/Parallax")]
    public class Parallax : DuMonoBehaviour
    {
        public enum ParallaxControl
        {
            Manual = 0,
            Time = 1,
            ParallaxController = 2,
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
        private ParallaxController m_ParallaxController = null;
        public ParallaxController parallaxController
        {
            get => m_ParallaxController;
            set => m_ParallaxController = value;
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
        private float m_Speed = 1f;
        public float speed
        {
            get => m_Speed;
            set => m_Speed = value;
        }

        [SerializeField]
        private float m_TileLength = 1f;
        public float tileLength
        {
            get => m_TileLength;
            set => m_TileLength = value;
        }

        [SerializeField]
        private float m_TileOffset = 0.5f;
        public float tileOffset
        {
            get => m_TileOffset;
            set => m_TileOffset = NormalizeTileOffset(value);
        }

        [SerializeField]
        private GameObject m_TileObject;
        public GameObject tileObject
        {
            get => m_TileObject;
            set => m_TileObject = value;
        }

        [SerializeField]
        private int m_TilesCount = 3;
        public int tilesCount
        {
            get => m_TilesCount;
            set => m_TilesCount = NormalizeTilesCount(value);
        }

        [SerializeField]
        private UpdateMode m_UpdateMode = UpdateMode.Update;
        public UpdateMode updateMode
        {
            get => m_UpdateMode;
            set => m_UpdateMode = value;
        }

        [SerializeField]
        private GizmoVisibility m_GizmoVisibility = GizmoVisibility.DrawOnSelect;
        public GizmoVisibility gizmoVisibility
        {
            get => m_GizmoVisibility;
            set => m_GizmoVisibility = value;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        [SerializeField]
        private List<ParallaxInstance> m_Instances = new List<ParallaxInstance>();

        private float m_OffsetDynamic;

        //--------------------------------------------------------------------------------------------------------------

        void Update()
        {
            if (parallaxControl == ParallaxControl.ParallaxController)
            {
                if (Dust.IsNull(parallaxController))
                    return;

                if (parallaxController.updateMode != UpdateMode.Update)
                    return;
            }
            else if (updateMode != UpdateMode.Update)
                return;

            UpdateState(Time.deltaTime);
        }

        void LateUpdate()
        {
            if (parallaxControl == ParallaxControl.ParallaxController)
            {
                if (Dust.IsNull(parallaxController))
                    return;

                if (parallaxController.updateMode != UpdateMode.LateUpdate)
                    return;
            }
            else if (updateMode != UpdateMode.LateUpdate)
                return;

            UpdateState(Time.deltaTime);
        }

        void FixedUpdate()
        {
            if (parallaxControl == ParallaxControl.ParallaxController)
            {
                if (Dust.IsNull(parallaxController))
                    return;

                if (parallaxController.updateMode != UpdateMode.FixedUpdate)
                    return;
            }
            else if (updateMode != UpdateMode.FixedUpdate)
                return;

            UpdateState(Time.fixedDeltaTime);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        void UpdateOffset(float deltaTime)
        {
            if (freeze)
                return;

            switch (parallaxControl)
            {
                case ParallaxControl.Manual:
                    // Nothing need to do
                    break;

                case ParallaxControl.Time:
                    m_OffsetDynamic += deltaTime * timeScale * speed;
                    break;

                case ParallaxControl.ParallaxController:
                    if (Dust.IsNotNull(parallaxController))
                        m_OffsetDynamic = parallaxController.GetGlobalOffset() * speed;
                    break;

                default:
                    break;
            }
        }

        public void UpdateState(float deltaTime)
        {
            UpdateOffset(deltaTime);

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            float zeroOffset = -(offset + m_OffsetDynamic) + tileOffset * tileLength;
            float totalLength = tileLength * tilesCount;
            float centerOffset = totalLength / 2f;

            for (int i = 0; i < m_Instances.Count; i++)
            {
                var instance = m_Instances[i];

                if (Dust.IsNull(instance))
                    continue;

                float curTileOffset = zeroOffset + i * tileLength;
                curTileOffset = DuMath.Repeat(curTileOffset, totalLength);
                curTileOffset -= centerOffset;

                Vector3 position = new Vector3(curTileOffset, 0f, 0f);

                instance.transform.localPosition = position;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        public void RebuildTiles()
        {
            this.DestroyAllInstances();

            if (Dust.IsNull(tileObject))
                return;

            for (int i = 0; i < tilesCount; i++)
            {
                GameObject newGameObject;

#if UNITY_EDITOR
                if (Dust.IsPrefab(tileObject))
                {
                    newGameObject = PrefabUtility.InstantiatePrefab(tileObject) as GameObject;
                    newGameObject.transform.parent = this.transform;
                }
                else
#endif
                {
                    newGameObject = Instantiate(tileObject, this.transform);
                }

                var newInstance = newGameObject.AddComponent<ParallaxInstance>();
                newInstance.Initialize(this);

                m_Instances.Add(newInstance);
            }

            UpdateState(0f);
        }

        private void DestroyAllInstances()
        {
            int index = this.transform.childCount - 1;

            while (index >= 0)
            {
                ParallaxInstance instance = transform.GetChild(index).gameObject.GetComponent<ParallaxInstance>();

                if (Dust.IsNotNull(instance) && instance.parentParallax == this)
                    Dust.DestroyObjectWhenReady(instance.gameObject);

                index--;
            }

            this.m_Instances.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Selection.activeGameObject == this.gameObject)
                return;

            if (gizmoVisibility != GizmoVisibility.AlwaysDraw)
                return;

            DrawFieldGizmos(Color.gray);
        }

        private void OnDrawGizmosSelected()
        {
            if (gizmoVisibility == GizmoVisibility.Hidden)
                return;

            DrawFieldGizmos(Color.blue);
        }

        private void DrawFieldGizmos(Color color)
        {
            Vector3 visibleSize = new Vector3(tileLength * (tilesCount - 1), 1000000f, 0.001f);

            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = color;
            Gizmos.DrawWireCube(transform.position, visibleSize);
        }
#endif

        //--------------------------------------------------------------------------------------------------------------

        private void Reset()
        {
            DestroyAllInstances();
        }

        //--------------------------------------------------------------------------------------------------------------
        // Normalizer

        public static float NormalizeTileOffset(float value)
        {
            return Mathf.Clamp01(value);
        }

        public static int NormalizeTilesCount(int value)
        {
            return Mathf.Max(2, value);
        }
    }
}
