using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    [RequireComponent(typeof(CameraRaycaster))]
    public class CameraMovement : MonoBehaviour 
    {
        #region Private Fields

        private Vector2 _dragStartedPosition;

        private Vector3 _moveStartPosition;

        private Vector3 _zoomMoveTargetPosition;

        private float _screenResolutionCoefficient = 1;

        private bool _dragStarted;

        private bool _zoomInStarted;

        private float _timeToTravel;

        private float _zoomForce;

        private CameraRaycaster _cameraRaycaster;

        private CancellationTokenSource _zoomMoveCancelTokenSrc;

        private CancellationToken _zoomMoveCancelToken;

        #endregion
        
        #region Injected Fields

        [Inject] private SignalBus _signalBus;

        #endregion

        #region Serialized Fields

        [SerializeField] private float _moveSpeed = 1f;
        
        [SerializeField] private float _zoomSpeed = 1f;
        
        [SerializeField] private float _zoomMaxSpeed = 1f;
        
        [SerializeField] private float _zoomSpeedDecreaseSpeed = 1f;

        #endregion

        #region Accessors

        private CameraRaycaster CameraRaycaster => this.GetCachedComponent(ref _cameraRaycaster);

        private CancellationTokenSource ZoomMoveCancelTokenSrc => CommonUtils.GetCached(ref _zoomMoveCancelTokenSrc, () => new CancellationTokenSource());

        #endregion

        #region Unity Events

        private void OnEnable()
        {
            CrazyPawnsInput.Zoom += InputOnZoom;
            _signalBus.Subscribe<ISimpleDragStartedSignal>(OnSimpleDragStarted);
            _signalBus.Subscribe<ISimpleDragSignal>(OnSimpleDrag);
            _signalBus.Subscribe<ISimpleDragFinishedSignal>(OnSimpleDragFinished);
        }

        private void OnDisable() 
        {
            CrazyPawnsInput.Zoom -= InputOnZoom;
            _signalBus.Unsubscribe<ISimpleDragStartedSignal>(OnSimpleDragStarted);
            _signalBus.Unsubscribe<ISimpleDragSignal>(OnSimpleDrag);
            _signalBus.Unsubscribe<ISimpleDragFinishedSignal>(OnSimpleDragFinished);
            
            _zoomMoveCancelToken.ThrowIfCancellationRequested();
        }

        #endregion

        #region Class Implementation

        private void OnSimpleDragStarted(ISimpleDragStartedSignal signal) 
        {
            CalculateScreenCoefficient();
            _zoomMoveCancelToken.ThrowIfCancellationRequested();
            _zoomInStarted = false;
            
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

        private void InputOnZoom(Vector2 mousePosition, float zoomDelta) 
        {
            // Не учитываем зум пока камера двигается по драгу
            if (_dragStarted) 
            {
                return;
            }
            if (CameraRaycaster.PlaneCast(mousePosition, out var point, out var distance)) 
            {
                if (Mathf.Approximately(zoomDelta, 0)) 
                {
                    return;
                }
                _moveStartPosition = transform.position;
                _zoomMoveTargetPosition = point;
                if (!_zoomInStarted) 
                {
                    _zoomInStarted = true;
                    _zoomMoveCancelToken = ZoomMoveCancelTokenSrc.Token;
                    ZoomMove(_zoomMoveCancelToken);
                }
                
                var sign = Mathf.Sign(zoomDelta);
                if (!Mathf.Approximately(sign, Mathf.Sign(_zoomForce))) 
                {
                    _zoomForce = sign > 0 ? Mathf.Min(_zoomMaxSpeed, _zoomForce + _zoomSpeed) : Mathf.Max(-1 * _zoomMaxSpeed, _zoomForce + -1 * _zoomSpeed);
                } 
                else 
                {
                    _zoomForce = _zoomSpeed * sign;
                }
            }
        }

        private async UniTask ZoomMove(CancellationToken cancellationToken) 
        {
            while ((transform.position - _zoomMoveTargetPosition).magnitude > 0.1f && !Mathf.Approximately(_zoomForce, 0)) 
            {
                var direction = (_zoomMoveTargetPosition - transform.position).normalized;
                transform.position += _zoomForce * Time.deltaTime * direction;
                _zoomForce += Mathf.Sign(_zoomForce) * -1 * _zoomSpeedDecreaseSpeed * Time.deltaTime;
                await UniTask.Yield(cancellationToken);
            }

            _zoomInStarted = false;
        }
        
        #endregion
    }
}