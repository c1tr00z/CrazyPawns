using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class Pawn : MonoBehaviour, IConnectorParent
    {
        #region Events

        private PawnStateChangedSignal _stateChangedSignal;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings _implementationSettings;
        
        [Inject] private IAssetProvider _assetProvider;

        [Inject] private SignalBus _signalBus;

        #endregion
        
        #region Serialized Fields

        [SerializeField] private Material _materialValid;
        
        [SerializeField] private Material _materialInvalid;

        [SerializeField] private MeshRenderer Body;

        #endregion

        #region Accessors

        private PawnStateChangedSignal StateChangedSignal => CommonUtils.GetCached(ref _stateChangedSignal, () => new PawnStateChangedSignal(this));

        public PawnState State { get; private set; } = PawnState.Valid;

        public Material CurrentMaterial => State == PawnState.Valid ? _materialValid : _materialInvalid;
        
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
            _signalBus.Fire(StateChangedSignal);
        }

        #endregion
    }
}