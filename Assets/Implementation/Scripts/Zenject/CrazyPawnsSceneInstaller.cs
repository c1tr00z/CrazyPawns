using Zenject;
namespace CrazyPawn.Implementation 
{
    public class CrazyPawnsSceneInstaller : MonoInstaller 
    {
        #region MonoInstaller Implementation

        public override void InstallBindings()
        {
            //Signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<IStateChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<IPawnConnectorActivate>().OptionalSubscriber();
            Container.DeclareSignal<IPawnConnectorDeactivate>().OptionalSubscriber();
            Container.DeclareSignal<IPawnDragStartedSignal>().OptionalSubscriber();
            Container.DeclareSignal<IPawnDraggedSignal>().OptionalSubscriber();
            Container.DeclareSignal<IPawnDragFinishedSignal>().OptionalSubscriber();
            Container.DeclareSignal<IPawnRemovedSignal>().OptionalSubscriber();
            Container.DeclareSignal<ISimpleDragStartedSignal>().OptionalSubscriber();
            Container.DeclareSignal<ISimpleDragSignal>().OptionalSubscriber();
            Container.DeclareSignal<ISimpleDragFinishedSignal>().OptionalSubscriber();
            
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