using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation.Ui 
{
    public class PawnsManagementUiFrame : MonoBehaviour 
    {
        #region Injected Fields

        [Inject] private IPawnCreator _pawnCreator;
        [Inject] private IPawnPooler _pawnPooler;
        
        #endregion
        
        #region Class Implementation

        public void SpawnPawn() 
        {
            _pawnCreator.Create();
        }

        public void RespawnPawns() 
        {
            _pawnPooler.ReturnAllToPool();
            _pawnCreator.CreatePawns();
        }

        #endregion
    }
}