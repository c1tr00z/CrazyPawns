using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public class MouseConnection : ConnectionBase
    {
        #region Private Fields

        private PawnConnector _connector;

        private Vector3 _mouseWorldPosition;

        #endregion
        
        #region ConnectionBase Implementation

        protected override Vector3[] VectorPoints => new[] {
            _connector.transform.position, _mouseWorldPosition
        };

        #endregion

        #region Class Implementation

        public void SetConnector(PawnConnector connector) 
        {
            _connector = connector;
        }
        
        public void UpdateMouse3DPosition(Vector3 mouse3dPosition) 
        {
            _mouseWorldPosition = mouse3dPosition;
        }

        #endregion
    }
}