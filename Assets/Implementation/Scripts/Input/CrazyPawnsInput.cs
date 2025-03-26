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
        
        #region Private Fields

        private readonly float _holdThreshold = 0.25f;

        private float _mouseDownTime;

        private bool _isPressed;
        
        private bool _holdStarted;

        #endregion

        #region Accessors

        private Vector2 CurrentMousePosition => Mouse.current.position.value;

        #endregion

        #region Unity Events

        private void LateUpdate() 
        {
            if (!_isPressed) 
            {
                return;    
            }
            if (Time.time - _mouseDownTime < _holdThreshold) 
            {
                return;
            }
            if (!_holdStarted) {
                _holdStarted = true;
                DragStarted?.Invoke(CurrentMousePosition);
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

        #endregion
    }
}