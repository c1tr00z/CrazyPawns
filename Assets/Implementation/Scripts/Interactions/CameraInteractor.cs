using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    [RequireComponent(typeof(Camera))]
    public class CameraInteractor : MonoBehaviour 
    {
        #region Private Fields

        private SignalBus _signalBus;

        private Camera _cameraObject;

        private Pawn _activePawn;

        private PawnConnector _activeConnector;

        private Vector2 _boardMinCoord;
        
        private Vector2 _boardMaxCoord;

        private PawnProviderSignal _pawnDraggedSignal;

        private ConnectorProviderSignal _connectorProviderSignal;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnSettings _crazyPawnSettings;
        
        [Inject] private CrazyPawnsImplSettings _implementationSettings;

        [Inject] private IPawnPooler _pawnPooler;

        #endregion

        #region Accessors

        private Camera Camera => this.GetCachedComponent(ref _cameraObject);

        private PawnProviderSignal PawnProviderSignal => CommonUtils.GetCached(ref _pawnDraggedSignal, () => new PawnProviderSignal());

        private ConnectorProviderSignal ConnectorProviderSignal => CommonUtils.GetCached(ref _connectorProviderSignal, () => new ConnectorProviderSignal());

        #endregion
        
        #region Unity Events

        private void OnEnable() 
        {
            CrazyPawnsInput.Tap += InputOnTap;
            CrazyPawnsInput.DragStarted += InputOnDragStarted;
            CrazyPawnsInput.Drag += CrazyPawnsInputOnDrag;
            CrazyPawnsInput.DragFinished += InputOnDragFinished;
        }

        private void OnDisable() 
        {
            CrazyPawnsInput.Tap -= InputOnTap;
            CrazyPawnsInput.DragStarted -= InputOnDragStarted;
            CrazyPawnsInput.Drag -= CrazyPawnsInputOnDrag;
            CrazyPawnsInput.DragFinished -= InputOnDragFinished;
        }

        private void Start() 
        {
            var boardSideSize = _crazyPawnSettings.CheckerboardSize * _implementationSettings.CheckerboardSquareSize;
            var boardHalfSize = boardSideSize / 2;
            _boardMinCoord = new Vector2(
                -boardHalfSize,
                -boardHalfSize
                );
            _boardMaxCoord = new Vector2(
                boardHalfSize, 
                boardHalfSize
                );
        }

        #endregion

        #region Zenject Events

        [Inject]
        public void Construct(SignalBus signalBus) 
        {
            _signalBus = signalBus;
        }

        #endregion

        #region Class Implementation

        private void InputOnTap(Vector2 newPosition) 
        {
            var ray = Camera.ScreenPointToRay(newPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, LayerMask.GetMask("Connectors"))) {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Connectors")) 
                {
                    var connector = hitInfo.transform.GetComponent<PawnConnector>();
                    if (connector is null) {
                        return;
                    }
                    if (_activeConnector is null) 
                    {
                        _activeConnector = connector;
                        FireConnectorActivateSignal(connector);
                    } 
                    else 
                    {
                        FireConnectorDeactivateSignal(connector);
                        _activeConnector = null;
                    }
                }
            }
            else 
            {
                FireConnectorDeactivateSignal(null);
                _activeConnector = null;
            }
        }

        private void InputOnDragStarted(Vector2 newPosition) 
        {
            var ray = Camera.ScreenPointToRay(newPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, LayerMask.GetMask("Connectors", "Pawn"))) {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Pawn")) 
                {
                    var pawn = hitInfo.transform.GetComponentInParent<Pawn>();
                    if (pawn is null) {
                        return;
                    }
                    _activePawn = pawn;
                    return;
                }
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Connectors")) 
                {
                    var connector = hitInfo.transform.GetComponent<PawnConnector>();
                    if (connector is null) {
                        return;
                    }
                    _activeConnector = connector;
                    FireConnectorActivateSignal(connector);
                }
            }
        }

        private void CrazyPawnsInputOnDrag(Vector2 newPosition) {
            if (_activePawn is null) {
                return;
            }
            var ray = Camera.ScreenPointToRay(newPosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                var point = ray.GetPoint(distance);
                _activePawn.transform.position = point;
                _activePawn.SetState(IsPointInsideBoard(point) ? PawnState.Valid : PawnState.Invalid);
                if (PawnProviderSignal.Pawn != _activePawn) 
                {
                    PawnProviderSignal.UpdatePawn(_activePawn);
                }
                _signalBus.Fire<IPawnDraggedSignal>(PawnProviderSignal);
            }
        }

        private void InputOnDragFinished(Vector2 newPosition) 
        {
            if (_activePawn is not null && !IsPointInsideBoard(_activePawn.transform.position))
            {
                _pawnPooler.ReturnToPool(_activePawn);
            }

            var ray = Camera.ScreenPointToRay(newPosition);
            PawnConnector connector = null;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, LayerMask.GetMask("Connectors"))) {
                connector = hitInfo.transform.GetComponent<PawnConnector>();
            }
            FireConnectorDeactivateSignal(connector);
            _activeConnector = null;
            _activePawn = null;
        }

        private bool IsPointInsideBoard(Vector3 point) 
        {
            return point.x >= _boardMinCoord.x && point.z >= _boardMinCoord.y && point.x <= _boardMaxCoord.x && point.z <= _boardMaxCoord.y;
        }

        private void FireConnectorActivateSignal(PawnConnector connector)
        {
            if (ConnectorProviderSignal.Connector != connector) 
            {
                ConnectorProviderSignal.UpdateConnector(connector);
            }
            
            _signalBus.Fire<IPawnConnectorActivate>(ConnectorProviderSignal);
        }

        private void FireConnectorDeactivateSignal(PawnConnector connector)
        {
            if (ConnectorProviderSignal.Connector != connector) 
            {
                ConnectorProviderSignal.UpdateConnector(connector);
            }
            
            _signalBus.Fire<IPawnConnectorDeactivate>(ConnectorProviderSignal);
        }

        #endregion
    }
}