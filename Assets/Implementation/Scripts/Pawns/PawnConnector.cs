using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class PawnConnector : MonoBehaviour
    {
        #region Private Fields

        private IConnectorParent _parentPawn;
        
        private Material _connectorActiveMaterial;

        private MeshRenderer _connectorRenderer;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings _implementationSettings;
        
        [Inject] private IAssetProvider _assetProvider;

        [Inject] private SignalBus _signalBus;
        
        #endregion

        #region Serialized Fields

        [SerializeField] private Material _materialConnectorActive;

        #endregion

        #region Accessors

        public IConnectorParent Parent => CommonUtils.GetCached(ref _parentPawn, GetComponentInParent<IConnectorParent>);

        private MeshRenderer MeshRenderer => this.GetCachedComponent(ref _connectorRenderer);

        #endregion

        #region Unity Events

        private void OnEnable() 
        {
            _signalBus.Subscribe<PawnStateChangedSignal>(OnParentStateChanged);
            _signalBus.Subscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Subscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        private void OnDisable() 
        {
            _signalBus.Unsubscribe<PawnStateChangedSignal>(OnParentStateChanged);
            _signalBus.Unsubscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Unsubscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        #endregion

        #region Class Implementation
        
        private void OnParentStateChanged(PawnStateChangedSignal signal) 
        {
            if (signal.StateProvider is not IConnectorParent otherParent || Parent != otherParent) 
            {
                return;
            }
            MeshRenderer.sharedMaterial = Parent.CurrentMaterial;
        }

        private void OnConnectorActivate(IPawnConnectorActivate signal) 
        {
            if (signal.Connector.Parent == Parent) 
            {
                return;
            }

            MeshRenderer.sharedMaterial = _materialConnectorActive;
        }

        private void OnConnectorDeactivate(IPawnConnectorDeactivate signal) 
        {
            MeshRenderer.sharedMaterial = Parent.CurrentMaterial;
        }

        #endregion
    }
}