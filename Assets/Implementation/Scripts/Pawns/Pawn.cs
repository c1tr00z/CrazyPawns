using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class Pawn : MonoBehaviour 
    {
        #region Events

        public event Action StateChanged;

        #endregion
        
        #region Private Fields

        private Material _validStateMaterial;
        
        private Material _invalidStateMaterial;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings _implementationSettings;
        
        [Inject] private IAssetProvider _assetProvider;

        [Inject] private SignalBus _signalBus;

        #endregion
        
        #region Serialized Fields

        [SerializeField] private MeshRenderer Body;

        #endregion

        #region Accessors

        public PawnState State { get; private set; } = PawnState.Valid;

        private Material MaterialValid =>
            CommonUtils.GetCached(ref _validStateMaterial, () => _assetProvider.ProvideAssetByKey<Material>(_implementationSettings.PawnMaterialValidKey));

        private Material MaterialInvalid =>
            CommonUtils.GetCached(ref _invalidStateMaterial, () => _assetProvider.ProvideAssetByKey<Material>(_implementationSettings.PawnMaterialInvalidKey));

        public Material CurrentMaterial => State == PawnState.Valid ? MaterialValid : MaterialInvalid;
        
        #endregion

        #region Class Implementation

        public void SetState(PawnState newState) 
        {
            if (State == newState) 
            {
                return;
            }
            State = newState;
            Body.sharedMaterial = CurrentMaterial;
            StateChanged?.Invoke();
        }

        #endregion
    }
}