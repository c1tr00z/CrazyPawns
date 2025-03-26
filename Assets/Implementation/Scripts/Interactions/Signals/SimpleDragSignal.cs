using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public class SimpleDragSignal : ISimpleDragStartedSignal, ISimpleDragFinishedSignal
    {
        #region Accessors

        public Vector2 MousePosition { get; private set; }

        #endregion

        #region Class Implementation

        public void UpdateMousePosition(Vector2 newMousePosition) 
        {
            MousePosition = newMousePosition;
        }

        #endregion
    }
}