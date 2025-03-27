using UnityEngine;
namespace CrazyPawn.Implementation 
{
    [RequireComponent(typeof(Camera))]
    public class CameraRaycaster : MonoBehaviour 
    {
        #region Private Fields

        private Camera _camera;

        #endregion

        #region Accessors

        private Camera Camera => this.GetCachedComponent(ref _camera);

        #endregion

        #region Class Implementation

        public bool PlaneCast(Vector2 screenPosition, out Vector3 point, out float distance) 
        {
            var ray = Camera.ScreenPointToRay(screenPosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }
            distance = 0;
            point = Vector3.zero;
            return false;
        }

        public bool Raycast(Vector2 screenPosition, LayerMask layerMask, out Transform hitTransform) 
        {
            var ray = Camera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, layerMask)) {
                hitTransform = hitInfo.transform;
                return true;
            }
            hitTransform = null;
            return false;
        }

        #endregion
    }
}