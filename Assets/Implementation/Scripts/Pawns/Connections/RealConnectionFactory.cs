using CrazyPawn.Implementation;
using Zenject;
namespace CrazyPawn.Implementation {
    public class RealConnectionFactory : IFactory<Connection> {
        
        #region Private Fields

        private Connection _connectionPrefabObject;

        private DiContainer _container;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IAssetProvider AssetProvider;
        
        #endregion

        #region Accessors

        private Connection ConnectionPrefab =>
            CommonUtils.GetCached(ref _connectionPrefabObject, 
                () => AssetProvider.ProvideAssetByKey<Connection>(ImplementationSettings.ConnectionPrefabResourceKey));

        #endregion

        #region Constructors

        public RealConnectionFactory(DiContainer container) 
        {
            _container = container;
        }

        #endregion
        
        #region IFactory Implementation

        public Connection Create() 
        {
            return _container.InstantiatePrefabForComponent<Connection>(ConnectionPrefab);
        }

        #endregion
    }
}