using Zenject;
namespace CrazyPawn.Implementation {
    public class RealPawnsFactory : IFactory<Pawn> {

        #region Private Fields

        private Pawn PawnPrefabObject;

        private DiContainer Container;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IAssetProvider AssetProvider;
        
        #endregion

        #region Accessors

        private Pawn PawnPrefab =>
            CommonUtils.GetCached(ref PawnPrefabObject, () => AssetProvider.ProvideAssetByKey<Pawn>(ImplementationSettings.PawnPrefabResourceKey));

        #endregion

        #region Constructors

        public RealPawnsFactory(DiContainer container) 
        {
            Container = container;
        }

        #endregion
        
        #region IFactory Implementation

        public Pawn Create() 
        {
            return Container.InstantiatePrefabForComponent<Pawn>(PawnPrefab);
        }

        #endregion
    }
}