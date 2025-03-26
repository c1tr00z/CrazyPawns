using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class PawnsManager : IPawnCreator, IPawnPooler {

        #region Private Fields

        private SignalBus _signalBus;

        private Queue<Pawn> _cached = new();
        
        private Transform _pool;

        private PawnProviderSignal _pawnRemovedSignal;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnSettings _crazyPawnSettings;

        [Inject] private PawnFactory _pawnFactory;

        #endregion

        #region Accessors

        private PawnProviderSignal PawnRemovedSignal => CommonUtils.GetCached(ref _pawnRemovedSignal, () => new PawnProviderSignal());

        #endregion
        
        #region Constructors

        [Inject]
        public PawnsManager(SignalBus signalBus) 
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<IStateChangedSignal>(OnStateChanged);
            _signalBus.Subscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region Destructors

        ~PawnsManager()
        {
            _signalBus.Unsubscribe<IStateChangedSignal>(OnStateChanged);
            _signalBus.Unsubscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region IPawnCreator Implementation

        public Pawn Create() 
        {
            Pawn pawn = null;
            if (_cached.Count > 0) 
            {
                pawn = _cached.Dequeue();
                pawn.transform.parent = null;
            } 
            else 
            {
                pawn = _pawnFactory.Create();
            }
            pawn.SetState(PawnState.Valid);
            PlacePawn(pawn);
            return pawn;
        }

        #endregion

        #region IPawnPooler Implementation

        public void ReturnToPool(Pawn pawn)
        {
            if (pawn is null) 
            {
                return;
            }
            if (_pool is null) 
            {
                _pool = new GameObject("PawnsPool").transform;
                _pool.gameObject.SetActive(false);
            }
            if (_cached.Contains(pawn))
            {
                return;
            }
            
            _cached.Enqueue(pawn);
            pawn.transform.parent = _pool;
            
            if (PawnRemovedSignal.Pawn != pawn) 
            {
                PawnRemovedSignal.UpdatePawn(pawn);
            }
            _signalBus.Fire<IPawnRemovedSignal>(PawnRemovedSignal);
        }

        #endregion

        #region Class Implementation

        private void OnStateChanged(IStateChangedSignal stateChanged) 
        {
            if (stateChanged.State != State.PawnsInit)
            {
                return;
            }
            
            SpawnPawns();
        }
        
        private void SpawnPawns() 
        {
            for (int i = 0; i < _crazyPawnSettings.InitialPawnCount; i++) 
            {
                Create();
            }
        }

        private void PlacePawn(Pawn pawn) {
            var radius = Random.Range(0, _crazyPawnSettings.InitialZoneRadius);
            var angle = Random.Range(0, 360);
            var point = new Vector3(
                radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                radius * Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            pawn.transform.position = point;
        }

        private void OnPawnRemoved(IPawnRemovedSignal signal) 
        {
            ReturnToPool(signal.Pawn);
        }

        #endregion
    }
}