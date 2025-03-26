using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class PawnConnector : MonoBehaviour
    {
        #region Private Fields

        private Pawn _parentPawn;
        
        private Material _connectorActiveMaterial;

        private MeshRenderer _connectorRenderer;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings _implementationSettings;
        
        [Inject] private IAssetProvider _assetProvider;

        [Inject] private SignalBus _signalBus;
        
        #endregion

        #region Accessors

        public Pawn Parent => CommonUtils.GetCached(ref _parentPawn, GetComponentInParent<Pawn>);
        
        private Material MaterialConnectorActive => CommonUtils.GetCached(ref _connectorActiveMaterial,
            () => _assetProvider.ProvideAssetByKey<Material>(_implementationSettings.ConnectorMaterialActiveKey));

        private MeshRenderer MeshRenderer => this.GetCachedComponent(ref _connectorRenderer);

        #endregion

        #region Unity Events

        private void OnEnable() 
        {
            Parent.StateChanged += OnParentStateChanged;
            _signalBus.Subscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Subscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        private void OnDisable() 
        {
            Parent.StateChanged -= OnParentStateChanged;
            _signalBus.Unsubscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Unsubscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        #endregion

        #region Class Implementation
        
        private void OnParentStateChanged() 
        {
            MeshRenderer.sharedMaterial = Parent.CurrentMaterial;
        }

        private void OnConnectorActivate(IPawnConnectorActivate signal) 
        {
            if (signal.Connector.Parent == Parent) 
            {
                return;
            }

            MeshRenderer.sharedMaterial = MaterialConnectorActive;
        }

        private void OnConnectorDeactivate(IPawnConnectorDeactivate signal) 
        {
            MeshRenderer.sharedMaterial = Parent.CurrentMaterial;
        }

        #endregion
    }
}