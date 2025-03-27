using UnityEngine;
namespace CrazyPawn.Implementation 
{
    /// <summary>
    /// Компонент для рейкастов и плейнкастов через камеру
    /// </summary>
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

        public bool PlaneCast(Vector2 screenPosition, float yOffset, out Vector3 point) 
        {
            var ray = Camera.ScreenPointToRay(screenPosition);
            var plane = new Plane(Vector3.up, Vector3.zero + new Vector3(0, yOffset, 0));
            if (plane.Raycast(ray, out float distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }
            point = Vector3.zero;
            return false;
        }
        
        public bool PlaneCast(Vector2 screenPosition, out Vector3 point) 
        {
            return PlaneCast(screenPosition, 0, out point);
        }

        public bool Raycast(Vector2 screenPosition, LayerMask layerMask, out Transform hitTransform, out Vector3 point) 
        {
            var ray = Camera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, layerMask)) {
                hitTransform = hitInfo.transform;
                point = hitInfo.point;
                return true;
            }
            point = Vector3.zero;
            hitTransform = null;
            return false;
        }

        #endregion
    }
}