using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class CameraInteractor : MonoBehaviour 
    {

        #region Private Fields

        private SignalBus SignalBus;

        private Camera CameraObject;

        private Pawn ActivePawn;

        private Vector2 BoardMinCoord;
        
        private Vector2 BoardMaxCoord;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnSettings CrazyPawnSettings;
        
        [Inject] private CrazyPawnsImplSettings ImplementationSettings;

        [Inject] private IPawnPooler PawnPooler;

        #endregion

        #region Accessors

        private Camera Camera => CommonUtils.GetCached(ref CameraObject, GetComponent<Camera>);

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
            var boardSideSize = CrazyPawnSettings.CheckerboardSize * ImplementationSettings.CheckerboardSquareSize;
            var boardHalfSize = boardSideSize / 2;
            BoardMinCoord = new Vector2(
                -boardHalfSize,
                -boardHalfSize
                );
            BoardMaxCoord = new Vector2(
                boardHalfSize, 
                boardHalfSize
                );
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

        private void InputOnTap(Vector2 newPosition) 
        {
            throw new NotImplementedException();
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
                    ActivePawn = pawn;
                    return;
                }
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Connectors")) 
                {
                    var connector = hitInfo.transform.GetComponent<PawnConnector>();
                    if (connector is null) {
                        return;
                    }
                    SignalBus.Fire(PawnConnectorActivate.MakeNew(connector));
                }
            }
        }

        private void CrazyPawnsInputOnDrag(Vector2 newPosition) {
            if (ActivePawn is null) {
                return;
            }
            var ray = Camera.ScreenPointToRay(newPosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                var point = ray.GetPoint(distance);
                ActivePawn.transform.position = point;
                ActivePawn.SetState(IsPointInsideBoard(point) ? PawnState.Valid : PawnState.Invalid);
            }
        }

        private void InputOnDragFinished(Vector2 newPosition) 
        {
            if (ActivePawn is not null && !IsPointInsideBoard(ActivePawn.transform.position))
            {
                PawnPooler.ReturnToPool(ActivePawn);
            }

            var ray = Camera.ScreenPointToRay(newPosition);
            PawnConnector connector = null;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, LayerMask.GetMask("Connectors"))) {
                connector = hitInfo.transform.GetComponent<PawnConnector>();
            }
            SignalBus.Fire(PawnConnectorDeactivate.MakeNew(connector));
            ActivePawn = null;
        }

        private bool IsPointInsideBoard(Vector3 point) 
        {
            return point.x >= BoardMinCoord.x && point.z >= BoardMinCoord.y && point.x <= BoardMaxCoord.x && point.z <= BoardMaxCoord.y;
        }

        #endregion
    }
}