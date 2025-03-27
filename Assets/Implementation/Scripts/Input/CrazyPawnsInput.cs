using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class CrazyPawnsInput : MonoBehaviour 
    {
        public static event Action<Vector2> Tap;
        public static event Action<Vector2> DragStarted;
        public static event Action<Vector2> Drag;
        public static event Action<Vector2> DragFinished;
        public static event Action<Vector2, float> Zoom;
        public static event Action<Vector2> MouseMove; 
        
        #region Private Fields

        private float _mouseDownTime;

        private bool _isPressed;
        
        private bool _holdStarted;

        private Vector2 _mouseDownPosition;

        private Vector2 _prevMousePosition;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings _implementationSettings;

        #endregion

        #region Accessors

        private Vector2 CurrentMousePosition => Mouse.current.position.value;

        #endregion

        #region Unity Events

        private void LateUpdate() 
        {
            var currentMousePosition = CurrentMousePosition;
            if ((currentMousePosition - _prevMousePosition).magnitude > _implementationSettings.MouseMovementThreshold) 
            {
                _prevMousePosition = currentMousePosition;
                MouseMove?.Invoke(currentMousePosition);
            }
            if (!_isPressed) 
            {
                return;    
            }
            if ((currentMousePosition - _mouseDownPosition).magnitude < _implementationSettings.DragThreshold) 
            {
                return;
            }
            if (!_holdStarted) 
            {
                _holdStarted = true;
                DragStarted?.Invoke(currentMousePosition);
            }
            Drag?.Invoke(Mouse.current.position.value);
        }

        #endregion
        
        #region Class Implementation

        public void OnTap(InputAction.CallbackContext context) 
        {
            var newIsPressed = context.ReadValue<float>() > 0;
            if (_isPressed == newIsPressed) 
            {
                return;
            }
            _isPressed = newIsPressed;
            if (_isPressed) 
            {
                _mouseDownTime = Time.time;
                _mouseDownPosition = CurrentMousePosition;
            } 
            else 
            {
                _holdStarted = false;
                if (Time.time - _mouseDownTime < _implementationSettings.HoldThreshold)
                {
                    Tap?.Invoke(CurrentMousePosition);
                    return;
                }
                DragFinished?.Invoke(CurrentMousePosition);
            }
        }

        public void OnZoom(InputAction.CallbackContext context) 
        {
            Zoom?.Invoke(CurrentMousePosition, context.ReadValue<float>());
        }

        #endregion
    }
}