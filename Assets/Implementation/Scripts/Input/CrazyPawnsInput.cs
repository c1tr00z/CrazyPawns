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

        private readonly float _holdThreshold = 0.25f;

        private readonly float _dragThreshold = 1f;

        private readonly float _mouseMovementThreshold = 1;

        private float _mouseDownTime;

        private bool _isPressed;
        
        private bool _holdStarted;

        private Vector2 _mouseDownPosition;

        private Vector2 _prevMousePosition;

        #endregion

        #region Accessors

        private Vector2 CurrentMousePosition => Mouse.current.position.value;

        #endregion

        #region Unity Events

        private void LateUpdate() 
        {
            var currentMousePosition = CurrentMousePosition;
            if ((currentMousePosition - _prevMousePosition).magnitude > _mouseMovementThreshold) 
            {
                _prevMousePosition = currentMousePosition;
                MouseMove?.Invoke(currentMousePosition);
            }
            if (!_isPressed) 
            {
                return;    
            }
            if ((currentMousePosition - _mouseDownPosition).magnitude < _dragThreshold) 
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
                if (Time.time - _mouseDownTime < _holdThreshold)
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