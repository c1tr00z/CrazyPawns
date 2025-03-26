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

        private Material ValidStateMaterial;
        
        private Material InvalidStateMaterial;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IAssetProvider AssetProvider;

        [Inject] private SignalBus SignalBus;

        #endregion
        
        #region Serialized Fields

        [SerializeField] private MeshRenderer Body;

        #endregion

        #region Accessors

        public PawnState State { get; private set; } = PawnState.Valid;

        private Material MaterialValid =>
            CommonUtils.GetCached(ref ValidStateMaterial, () => AssetProvider.ProvideAssetByKey<Material>(ImplementationSettings.PawnMaterialValidKey));

        private Material MaterialInvalid =>
            CommonUtils.GetCached(ref InvalidStateMaterial, () => AssetProvider.ProvideAssetByKey<Material>(ImplementationSettings.PawnMaterialInvalidKey));

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