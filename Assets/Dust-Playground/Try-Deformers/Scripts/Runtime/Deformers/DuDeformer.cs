using UnityEditor;
using UnityEngine;

namespace DustEngine
{
    public abstract class DuDeformer : DuMonoBehaviour, IDynamicState
    {
        protected static readonly Color k_GizmosColorActive = new Color(1.00f, 0.60f, 0.60f);
        protected static readonly Color k_GizmosColorDisabled = new Color(0.60f, 0.60f, 0.60f);

        //--------------------------------------------------------------------------------------------------------------

        [SerializeField]
        protected FieldsMap m_FieldsMap = FieldsMap.Deformer();
        public FieldsMap fieldsMap => m_FieldsMap;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        [SerializeField]
        private GizmoVisibility m_GizmoVisibility = GizmoVisibility.DrawOnSelect;
        public GizmoVisibility gizmoVisibility
        {
            get => m_GizmoVisibility;
            set => m_GizmoVisibility = value;
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Start()
        {
            // Require to show enabled-checkbox in editor for all deformers
        }

        //--------------------------------------------------------------------------------------------------------------

        public virtual void UpdateMeshPointsCloud(ref Vector3[] pointsCloud, Transform meshTransform, float strength)
        {
            int pointsCount = pointsCloud.Length;

            Vector3 vLocalPosition; // vertex in local space of deformer object
            Vector3 vWorldPosition; // vertex in world space

            Matrix4x4 matrixMeshLocalToMeshWorld = meshTransform.localToWorldMatrix;
            Matrix4x4 matrixMeshWorldToDefmLocal = transform.worldToLocalMatrix;

            Matrix4x4 matrixDefmLocalToMeshWorld = transform.localToWorldMatrix;
            Matrix4x4 matrixMeshWorldToMeshLocal = meshTransform.worldToLocalMatrix;

            for (int i = 0; i < pointsCount; i++)
            {
                float deformPower = 1f;

                // 1. Transform vertex position: local-in-mesh > world
                // 2. Transform vertex position: world > local-in-deformer
                vWorldPosition = matrixMeshLocalToMeshWorld.MultiplyPoint(pointsCloud[i]);
                vLocalPosition = matrixMeshWorldToDefmLocal.MultiplyPoint(vWorldPosition);

                fieldsMap.Calculate(vWorldPosition, (float) i / pointsCount, out deformPower);

                if (DuMath.IsZero(deformPower))
                    continue;

                if (DeformPoint(ref vLocalPosition, deformPower * strength) == false)
                    continue;

                // 1. Back-Transform vertex position: local-in-deformer > world
                // 2. Back-Transform vertex position: world > local-in-mesh
                vWorldPosition = matrixDefmLocalToMeshWorld.MultiplyPoint(vLocalPosition);
                pointsCloud[i] = matrixMeshWorldToMeshLocal.MultiplyPoint(vWorldPosition);
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        public abstract string DeformerName();
        public abstract string DeformerDynamicHint();

        public abstract bool DeformPoint(ref Vector3 localPosition, float strength);

        //--------------------------------------------------------------------------------------------------------------
        // DynamicStateInterface

        public virtual int GetDynamicStateHashCode()
        {
            int seq = 0, dynamicState = 0;

            DynamicState.Append(ref dynamicState, ++seq, transform);
            DynamicState.Append(ref dynamicState, ++seq, fieldsMap);

            return DynamicState.Normalize(dynamicState);
        }

        //--------------------------------------------------------------------------------------------------------------
        // Helpers

        protected bool IsPointInsideDeformBox(Vector3 point, Vector3 size)
        {
            return -size.x / 2f <= point.x && point.x <= +size.x / 2f &&
                   -size.y / 2f <= point.y && point.y <= +size.y / 2f &&
                   -size.z / 2f <= point.z && point.z <= +size.z / 2f;
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (Selection.activeGameObject == this.gameObject)
                return;

            if (gizmoVisibility != GizmoVisibility.AlwaysDraw)
                return;

            DrawDeformerGizmos();
        }

        void OnDrawGizmosSelected()
        {
            if (gizmoVisibility == GizmoVisibility.Hidden)
                return;

            DrawDeformerGizmos();
        }

        protected abstract void DrawDeformerGizmos();
#endif
    }
}
