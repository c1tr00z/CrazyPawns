using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public interface IConnectionDragSignal
    {
        #region Accessors
        
        public Vector3 Mouse3dPosition { get; }

        public PawnConnector Connector { get; }

        #endregion
    }
}