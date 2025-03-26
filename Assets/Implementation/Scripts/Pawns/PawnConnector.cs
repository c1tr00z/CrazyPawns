using System;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class PawnConnector : MonoBehaviour
    {
        #region Private Fields

        private Pawn ParentPawn;
        
        private Material ConnecterActiveMaterial;

        private MeshRenderer ConnectorRenderer;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IAssetProvider AssetProvider;

        [Inject] private SignalBus SignalBus;
        
        #endregion

        #region Accessors

        public Pawn Parent => CommonUtils.GetCached(ref ParentPawn, GetComponentInParent<Pawn>);
        
        private Material MaterialConnectorActive => CommonUtils.GetCached(ref ConnecterActiveMaterial,
            () => AssetProvider.ProvideAssetByKey<Material>(ImplementationSettings.ConnectorMaterialActiveKey));

        private MeshRenderer MeshRenderer => this.GetCachedComponent(ref ConnectorRenderer);

        #endregion

        #region Unity Events

        private void OnEnable() 
        {
            Parent.StateChanged += OnParentStateChanged;
            SignalBus.Subscribe<PawnConnectorActivate>(OnConnectorActivate);
            SignalBus.Subscribe<PawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        private void OnDisable() 
        {
            Parent.StateChanged -= OnParentStateChanged;
            SignalBus.Unsubscribe<PawnConnectorActivate>(OnConnectorActivate);
            SignalBus.Unsubscribe<PawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        #endregion

        #region Class Implementation
        
        private void OnParentStateChanged() 
        {
            MeshRenderer.sharedMaterial = Parent.CurrentMaterial;
        }

        private void OnConnectorActivate(PawnConnectorActivate signal) 
        {
            if (signal.Connector.Parent == Parent) 
            {
                return;
            }

            MeshRenderer.sharedMaterial = MaterialConnectorActive;
        }

        private void OnConnectorDeactivate(PawnConnectorDeactivate signal) 
        {
            MeshRenderer.sharedMaterial = Parent.CurrentMaterial;
        }

        #endregion
    }
}