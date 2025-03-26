using Implementation.Scripts.Pawns.Connections;
using Zenject;
namespace CrazyPawn.Implementation 
{
    public class CrazyPawnsSceneInstaller : MonoInstaller 
    {
        #region MonoInstaller Implementation

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<StateChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<PawnConnectorActivate>().OptionalSubscriber();
            Container.DeclareSignal<PawnConnectorDeactivate>().OptionalSubscriber();
            Container.DeclareSignal<IPawnDraggedSignal>().OptionalSubscriber();
            Container.DeclareSignal<IPawnRemovedSignal>().OptionalSubscriber();
            Container.Bind<IInitializable>().To<SceneStarter>().AsSingle();
            Container.Bind<IAssetProvider>().To<AssetManager>().AsSingle();
            Container.Bind(typeof(IPawnCreator), typeof(IPawnPooler)).To<PawnsManager>().AsSingle().NonLazy();
            Container.BindFactory<Pawn, PawnFactory>().FromFactory<RealPawnsFactory>();
            Container.Bind(typeof(IStateCompleter), typeof(IStateProvider)).To<StateManager>().AsSingle();
            Container.Bind<IConnectionPooler>().To<ConnectionsManager>().AsSingle().NonLazy();
            Container.BindFactory<Connection, ConnectionFactory>().FromFactory<RealConnectionFactory>();
        }

        #endregion
    }
}