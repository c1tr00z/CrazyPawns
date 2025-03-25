using UnityEngine;
namespace CrazyPawn.Implementation
{
    public class InputSignal 
    {
        #region Accessors

        public InputEventType Type;

        public Vector2 ScreenPosition;

        #endregion

        #region Class Implementation

        public static InputSignal MakeNew(InputEventType type, Vector2 screenPosition)
        {
            return new InputSignal 
            {
                Type = type,
                ScreenPosition = screenPosition,
            };
        }

        #endregion
    }
}