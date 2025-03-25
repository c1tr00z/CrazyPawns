using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class Pawn : MonoBehaviour 
    {
        #region Private Fields

        private Material ValidStateMaterial;
        
        private Material InvalidStateMaterial;

        #endregion

        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IAssetProvider AssetProvider;

        #endregion
        
        #region Serialized Fields

        [SerializeField] private MeshRenderer Body;

        [SerializeField] private List<MeshRenderer> Connectors = new();

        #endregion

        #region Accessors

        public PawnState State { get; private set; } = PawnState.Valid;

        private Material MaterialValid =>
            CommonUtils.GetCached(ref ValidStateMaterial, () => AssetProvider.ProvideAssetByKey<Material>(ImplementationSettings.PawnMaterialValidKey));

        private Material MaterialInvalid =>
            CommonUtils.GetCached(ref InvalidStateMaterial, () => AssetProvider.ProvideAssetByKey<Material>(ImplementationSettings.PawnMaterialInvalidKey));
        
        #endregion

        #region Class Implementation

        public void SetState(PawnState newState) 
        {
            if (State == newState) 
            {
                return;
            }
            State = newState;
            var newMaterial = State == PawnState.Valid ? MaterialValid : MaterialInvalid;
            Body.sharedMaterial = newMaterial;
            Connectors.ForEach(c => c.sharedMaterial = newMaterial);
        }

        #endregion
    }
}