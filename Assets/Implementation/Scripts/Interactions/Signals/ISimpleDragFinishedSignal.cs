using UnityEngine;

namespace CrazyPawn.Implementation 
{
    public interface ISimpleDragFinishedSignal
    {
        #region Accessors

        public Vector2 MousePosition { get; }

        #endregion
    }
}