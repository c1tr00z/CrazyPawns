using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class PawnsManager : IPawnCreator, IPawnPooler {

        #region Private Fields

        private SignalBus SignalBus;

        private Queue<Pawn> Cached = new();
        
        private Transform Pool;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnSettings CrazyPawnSettings;

        [Inject] private PawnFactory PawnFactory;

        #endregion
        
        #region Constructors

        [Inject]
        public PawnsManager(SignalBus signalBus) 
        {
            SignalBus = signalBus;
            SignalBus.Subscribe<StateChangedSignal>(OnStateChanged);
        }

        #endregion

        #region Destructors

        ~PawnsManager()
        {
            SignalBus.Unsubscribe<StateChangedSignal>(OnStateChanged);
        }

        #endregion

        #region IPawnCreator Implementation

        public Pawn Create() 
        {
            Pawn pawn = null;
            if (Cached.Count > 0) 
            {
                pawn = Cached.Dequeue();
                pawn.transform.parent = null;
            } 
            else 
            {
                pawn = PawnFactory.Create();
            }
            PlacePawn(pawn);
            return pawn;
        }

        #endregion

        #region IPawnPooler Implementation

        public void ReturnToPool(Pawn pawn)
        {
            if (Pool is null) 
            {
                Pool = new GameObject("PawnsPool").transform;
                Pool.gameObject.SetActive(false);
            }
            if (Cached.Contains(pawn))
            {
                return;
            }
            
            Cached.Enqueue(pawn);
            pawn.transform.parent = Pool;
        }

        #endregion

        #region Class Implementation

        private void OnStateChanged(StateChangedSignal stateChanged) 
        {
            if (stateChanged.State != State.PawnsInit)
            {
                return;
            }
            
            SpawnPawns();
        }
        
        private void SpawnPawns() 
        {
            for (int i = 0; i < CrazyPawnSettings.InitialPawnCount; i++) 
            {
                Create();
            }
        }

        private void PlacePawn(Pawn pawn) {
            var radius = Random.Range(0, CrazyPawnSettings.InitialZoneRadius);
            var angle = Random.Range(0, 360);
            var point = new Vector3(
                radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                radius * Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            pawn.transform.position = point;
        }

        #endregion
    }
}