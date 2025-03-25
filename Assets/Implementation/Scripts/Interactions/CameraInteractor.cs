using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class CameraInteractor : MonoBehaviour {

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

        private void Start() {
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

        private void OnEnable() 
        {
            SignalBus.Subscribe<InputSignal>(OnInputSignal);
            CrazyPawnsInput.Drag += CrazyPawnsInputOnDrag;
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

        private void OnDisable() 
        {
            SignalBus.Unsubscribe<InputSignal>(OnInputSignal);
            CrazyPawnsInput.Drag -= CrazyPawnsInputOnDrag;    
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

        private void OnInputSignal(InputSignal inputSignal) 
        {
            switch (inputSignal.Type) {
                case InputEventType.HoldStarted:
                    var ray = Camera.ScreenPointToRay(inputSignal.ScreenPosition);
                    if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
                        var pawn = hitInfo.transform.GetComponentInParent<Pawn>();
                        if (pawn is null) {
                            Debug.LogError("no pawn");
                            return;
                        }
                        ActivePawn = pawn;
                    }
                    break;
                case InputEventType.HoldReleased:
                    if (ActivePawn is not null && !IsPointInsideBoard(ActivePawn.transform.position))
                    {
                        PawnPooler.ReturnToPool(ActivePawn);
                    }
                    ActivePawn = null;
                    break;
            }
        }

        private bool IsPointInsideBoard(Vector3 point) 
        {
            return point.x >= BoardMinCoord.x && point.z >= BoardMinCoord.y && point.x <= BoardMaxCoord.x && point.z <= BoardMaxCoord.y;
        }

        #endregion
    }
}