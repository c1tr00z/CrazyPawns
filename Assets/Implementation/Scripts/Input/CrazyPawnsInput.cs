using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class CrazyPawnsInput : MonoBehaviour 
    {
        public static event Action<Vector2> Drag; 
        
        #region Private Fields

        private readonly float HoldThreshold = 0.25f;

        private float _mouseDownTime;

        private bool IsPressed;
        
        private bool HoldStarted;
            
        private SignalBus SignalBus;

        #endregion

        #region Unity Events

        private void LateUpdate() {
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
                FireEvent(InputEventType.HoldStarted);
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
                    FireEvent(InputEventType.Tap);
                    return;
                } 
                else 
                {
                    FireEvent(InputEventType.HoldReleased);
                }
            }
        }

        private void FireEvent(InputEventType type) 
        {
            SignalBus.Fire(InputSignal.MakeNew(type, Mouse.current.position.value));
        }

        #endregion
    }
}