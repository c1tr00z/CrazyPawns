using Zenject;
namespace CrazyPawn.Implementation {
    public class RealPawnsFactory : IFactory<Pawn> {

        #region Private Fields

        private Pawn _pawnPrefabObject;

        private DiContainer _container;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings _implementationSettings;
        
        [Inject] private IAssetProvider _assetProvider;
        
        #endregion

        #region Accessors

        private Pawn PawnPrefab =>
            CommonUtils.GetCached(ref _pawnPrefabObject, () => _assetProvider.ProvideAssetByKey<Pawn>(_implementationSettings.PawnPrefabResourceKey));

        #endregion

        #region Constructors

        public RealPawnsFactory(DiContainer container) 
        {
            _container = container;
        }

        #endregion
        
        #region IFactory Implementation

        public Pawn Create() 
        {
            return _container.InstantiatePrefabForComponent<Pawn>(PawnPrefab);
        }

        #endregion
    }
}