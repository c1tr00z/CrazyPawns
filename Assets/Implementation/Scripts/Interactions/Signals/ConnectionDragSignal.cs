using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public class ConnectionDragSignal : IConnectionStartedSignal, IConnectionFinishedSignal
    {
        #region IConnectionStartedSignal IConnectionSignal IConnectionFinishedSignal Implementation

        public Vector3 Mouse3dPosition { get; private set; }
        
        public PawnConnector Connector { get; private set; }

        #endregion

        #region Class Implementation

        public void UpdateMouse3dPosition(Vector3 mouse3dPosition) 
        {
            Mouse3dPosition = mouse3dPosition;
        }

        public void UpdateConnector(PawnConnector connector) 
        {
            Connector = connector;
        }

        #endregion
    }
}