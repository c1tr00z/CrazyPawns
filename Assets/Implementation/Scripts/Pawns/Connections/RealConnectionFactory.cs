using CrazyPawn.Implementation;
using Zenject;
namespace Implementation.Scripts.Pawns.Connections {
    public class RealConnectionFactory : IFactory<Connection> {
        
        #region Private Fields

        private Connection ConnectionPrefabObject;

        private DiContainer Container;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IAssetProvider AssetProvider;
        
        #endregion

        #region Accessors

        private Connection ConnectionPrefab =>
            CommonUtils.GetCached(ref ConnectionPrefabObject, 
                () => AssetProvider.ProvideAssetByKey<Connection>(ImplementationSettings.ConnectionPrefabResourceKey));

        #endregion

        #region Constructors

        public RealConnectionFactory(DiContainer container) 
        {
            Container = container;
        }

        #endregion
        
        #region IFactory Implementation

        public Connection Create() 
        {
            return Container.InstantiatePrefabForComponent<Connection>(ConnectionPrefab);
        }

        #endregion
    }
}