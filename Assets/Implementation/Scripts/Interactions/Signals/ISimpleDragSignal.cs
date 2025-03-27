using UnityEngine;
namespace CrazyPawn.Implementation {
    public interface ISimpleDragSignal 
    {
        #region Accessors

        public Vector2 MousePosition { get; }

        #endregion
    }
}