using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class PawnsManager {

        #region Private Fields

        private SignalBus SignalBus;

        private Pawn PawnPrefabObject;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnSettings CrazyPawnSettings;

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private CrazyPawnsResourcesSettings ResourcesSettings;
        
        #endregion

        #region Accessors

        private Pawn PawnPrefab => CommonUtils.GetCached(ref PawnPrefabObject, () => Resources.Load<Pawn>(ResourcesSettings.PawnPath));

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
                SpawnPawn();
            }
        }

        private void SpawnPawn() 
        {
            var radius = Random.Range(0, CrazyPawnSettings.InitialZoneRadius);
            var angle = Random.Range(0, 360);
            var point = new Vector3(
                radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                radius * Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            var newPawn = Object.Instantiate(PawnPrefab);
            newPawn.transform.position = point;
        }

        #endregion
    }
}