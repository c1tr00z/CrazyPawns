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

        private readonly float HoldThreshold = 0.25f;

        private float _mouseDownTime;

        private bool IsPressed;
        
        private bool HoldStarted;
            
        private SignalBus SignalBus;

        #endregion

        #region Accessors

        private Vector2 CurrentMousePosition => Mouse.current.position.value;

        #endregion

        #region Unity Events

        private void LateUpdate() 
        {
            if (!IsPressed) 
            {
                return;    
            }
            if (Time.time - _mouseDownTime < HoldThreshold) 
            {
                return;
            }
            if (!HoldStarted) {
                HoldStarted = true;
                DragStarted?.Invoke(CurrentMousePosition);
            }
            Drag?.Invoke(Mouse.current.position.value);
        }

        #endregion
        
        #region Zenject Events

        [Inject]
        public void Construct(SignalBus signalBus) 
        {
            SignalBus = signalBus;
        }

        #endregion
        
        #region Class Implementation

        public void OnTap(InputAction.CallbackContext context) 
        {
            var newIsPressed = context.ReadValue<float>() > 0;
            if (IsPressed == newIsPressed) 
            {
                return;
            }
            IsPressed = newIsPressed;
            if (IsPressed) 
            {
                _mouseDownTime = Time.time;
            } 
            else 
            {
                HoldStarted = false;
                if (Time.time - _mouseDownTime < HoldThreshold)
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