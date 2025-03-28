using UnityEngine;

namespace CrazyPawn.Implementation 
{
    public interface ISimpleDragStartedSignal 
    {
        #region Accessors

        public Vector2 MousePosition { get; }

        #endregion
    }
}