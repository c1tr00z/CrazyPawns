using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class CameraMovement : MonoBehaviour 
    {
        #region Private Fields

        private Vector2 _dragStartedPosition;

        private Vector3 _moveStartPosition;

        private float _screenResolutionCoefficient = 1;

        private bool _dragStarted;

        #endregion
        
        #region Injected Fields

        [Inject] private SignalBus _signalBus;

        #endregion

        #region Serialized Fields

        [SerializeField] private float _moveSpeed = 1f;

        #endregion

        #region Unity Events

        private void OnEnable()
        {
            _signalBus.Subscribe<ISimpleDragStartedSignal>(OnSimpleDragStarted);
            _signalBus.Subscribe<ISimpleDragSignal>(OnSimpleDrag);
            _signalBus.Subscribe<ISimpleDragFinishedSignal>(OnSimpleDragFinished);
        }

        private void OnDisable() 
        {
            _signalBus.Unsubscribe<ISimpleDragStartedSignal>(OnSimpleDragStarted);
            _signalBus.Unsubscribe<ISimpleDragSignal>(OnSimpleDrag);
            _signalBus.Unsubscribe<ISimpleDragFinishedSignal>(OnSimpleDragFinished);
        }

        #endregion

        #region Class Implementation

        private void OnSimpleDragStarted(ISimpleDragStartedSignal signal) 
        {
            CalculateScreenCoefficient();
            
            _dragStartedPosition = signal.MousePosition;
            _moveStartPosition = transform.position;
            _dragStarted = true;
        }

        private void OnSimpleDrag(ISimpleDragSignal signal) 
        {
            if (!_dragStarted) 
            {
                return;
            }
            var screenDelta = (signal.MousePosition - _dragStartedPosition) * _screenResolutionCoefficient * -1;
            var position3dDelta = new Vector3(screenDelta.x * _moveSpeed, 0, screenDelta.y * _moveSpeed);
            transform.position = _moveStartPosition + position3dDelta;
        }
        
        private void OnSimpleDragFinished(ISimpleDragFinishedSignal signal) 
        {
            _dragStarted = false;
        }

        private void CalculateScreenCoefficient() 
        {
            _screenResolutionCoefficient = 1080f / Screen.height;
        }
        
        #endregion
    }
}