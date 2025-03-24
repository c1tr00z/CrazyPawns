using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class PawnsManager {

        #region Injected Fields

        [Inject] private CrazyPawnSettings CrazyPawnSettings;

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;

        // [Inject] private Pawn PawnPrefab;

        #endregion
        
        public PawnsManager() {
            // SpawnPawns();
        }

        #region Class Implementation

        private void SpawnPawns() {
            // Debug.LogError(PawnPrefab);
        }

        #endregion
    }
}